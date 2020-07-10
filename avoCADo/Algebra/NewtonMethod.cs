using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace avoCADo.Algebra
{
    public static class NewtonMethod
    {
        public static IList<Vector4> CalculateIntersectionPoints(IntersectionData data, Vector4 startingPoint, float knotDistance, float epsilon = 0.00001f)
        {
            var pointList = new List<Vector4>();
            pointList.Add(startingPoint);
            while (ContinueIntersectionTracing(data, startingPoint, pointList.Last(), knotDistance))
            {
                var point = CalculateNextPoint(data, pointList.Last(), knotDistance, epsilon);
                if (SurfaceConditions.ParametersInBounds(data, point))
                {
                    pointList.Add(point);
                }
                else
                {
                    break;
                }
            }
            //TODO: check if intersection is looped

            if(pointList.Count == 1 || pointList.First() != pointList.Last())
            {
                var data2 = new IntersectionData(data.q, data.p);
                var pointList2 = new List<Vector4>();
                pointList2.Add(startingPoint.Zwxy);

                while (ContinueIntersectionTracing(data2, startingPoint, pointList2.Last(), knotDistance))
                {
                    var point = CalculateNextPoint(data2, pointList2.Last(), knotDistance, epsilon);
                    if (SurfaceConditions.ParametersInBounds(data2, point))
                    {
                        pointList2.Add(point);
                    }
                    else
                    {
                        break;
                    }
                }

                //We go from startingPoint in both directions, thus second list needs to be reversed 
                //thus, correct order: pointList2.last -> startingPoint -> pointList.last
                pointList2.Reverse(); 
                pointList2.RemoveAt(pointList2.Count - 1);//remove startingPoint (it is already included in pointList)
                pointList2 = pointList2.Select(x => x.Zwxy).ToList();
                pointList2.AddRange(pointList);
                return pointList2;
            }

            return pointList;
        }

        private static bool ContinueIntersectionTracing(IntersectionData data, Vector4 startingPoint, Vector4 lastPoint, float knotDistance)
        {
            return SurfaceConditions.ParametersInBounds(data, lastPoint);
        }

        private static Vector4 CalculateNextPoint(IntersectionData data, Vector4 x0, float knotDistance, float epsilon)
        {
            var maxIterations = 100;
            var xCur = x0;
            var xPrev = x0;

            int i = 0;
            while(NewtonCondition(data, xPrev, xCur, x0, knotDistance, epsilon) && i < maxIterations)
            {
                xPrev = xCur;
                xCur = NewtonIteration(data, xCur, x0, knotDistance);
                i++;
            }

            return xCur; //TODO: throw exception when no solution was found
        }

        private static bool NewtonCondition(IntersectionData data, Vector4 xPrev, Vector4 xCur, Vector4 x0, float knotDistance, float epsilon)
        {
            return SurfaceConditions.ParametersInBounds(data, xCur) && //TODO : check condition
                 (Vector3.Dot(data.p.GetVertex(xCur.X, xCur.Y), data.q.GetVertex(xCur.Z, xCur.W)) > epsilon || 
                 Math.Abs(StepDistance(data, xCur, x0, knotDistance)) > epsilon); //TODO: tweak end condition, use previous approx.
        }

        private static Vector4 NewtonIteration(IntersectionData data, Vector4 xCur, Vector4 x0, float knotDistance)
        {
            var mat = NewtonMatrix(data, xCur, x0);
            var f = F(data, xCur, x0, knotDistance);
            var dx = LinearEquationSolver.Solve(mat, -f);

            return dx + xCur;
        }

        private static Vector4 F(IntersectionData data, Vector4 xCur, Vector4 x0, float knotDistance)
        {
            var p = data.p;
            var q = data.q;

            var u = xCur.X;
            var v = xCur.Y;
            var s = xCur.Z;
            var t = xCur.W;

            var posDiff = p.GetVertex(u, v) - q.GetVertex(s, t);
            var dir = StepDistance(data, xCur, x0, knotDistance);

            return new Vector4(posDiff, dir);
        }

        private static float StepDistance(IntersectionData data, Vector4 xCur, Vector4 x0, float knotDistance)
        {
            var p = data.p;

            var u = xCur.X;
            var v = xCur.Y;

            var u0 = x0.X;
            var v0 = x0.Y;

            return Vector3.Dot(p.GetVertex(u, v) - p.GetVertex(u0, v0), ICTangent(data, x0)) - knotDistance;
        }

        private static Vector3 ICTangent(IntersectionData data, Vector4 x)
        {
            return Vector3.Cross(data.p.Normal(x.X, x.Y), data.q.Normal(x.Z, x.W)).Normalized();
        }

        private static Matrix4 NewtonMatrix(IntersectionData data, Vector4 xCur, Vector4 x0)
        {
            //F(u,v,s,t) = P(u,v) - Q(s,t) [3 equations]
            //G(u,v,s,t) = <P(u,v) - p0, t> - knotDistance [1 equation]
            var p = data.p;
            var q = data.q;

            var u = xCur.X;
            var v = xCur.Y;
            var s = xCur.Z;
            var t = xCur.W;

            var tVec = ICTangent(data, x0);

            Matrix4 mat = new Matrix4();

            var Pu = p.DerivU(u, v);
            var Pv = p.DerivV(u, v);
            var Qs = q.DerivU(s, t);
            var Qt = q.DerivV(s, t);

            mat.Column0 = new Vector4(Pu, Vector3.Dot(Pu, tVec));
            mat.Column1 = new Vector4(Pv, Vector3.Dot(Pv, tVec));
            mat.Column2 = new Vector4(-Qs, 0.0f);
            mat.Column3 = new Vector4(-Qt, 0.0f);
            return mat;
        }
    }
}
