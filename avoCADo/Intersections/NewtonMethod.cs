using avoCADo.Intersections;
using avoCADo.ParametricObjects;
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
        private const int _intersectionPointsLimit = 2000;
        private const float _pointDistanceEpsilon = 0.01f;
        private const float _c0AngleThreshold = (float)Math.PI * 0.45f;

        public static IList<Vector4> CalculateIntersectionPoints(IntersectionData data, Vector4 startingPoint, float knotDistance, float epsilon = 0.00001f)
        {
            var pointList = new List<Vector4>();
            pointList.Add(startingPoint);
            while (ContinueIntersectionTracing(data, startingPoint, pointList, knotDistance))
            {
                var point = CalculateNextPoint(data, pointList.Last(), knotDistance, epsilon);
                if (SurfaceConditions.ParametersInBounds(data, point))
                {
                    if(pointList.Count > _intersectionPointsLimit)
                    {
                        throw new InvalidOperationException("Intersection contains too many points, choose different parameters");
                    }
                    if (PointsCloseEnough(data, point, _pointDistanceEpsilon) == false)
                    {
                        throw new InvalidOperationException("Intersection error, calculated points too far away, choose different parameters");
                    }
                    pointList.Add(point);
                }
                else
                {
                    pointList.Add(BoundaryFinder.FindBoundaryPoint(data, pointList.Last(), point));
                    break;
                }
            }

            if(pointList.Count == 1 || pointList.First() != pointList.Last())
            {
                var data2 = new IntersectionData(data.q, data.p);
                var pointList2 = Enumerable.Reverse(pointList).Select(x => x.Zwxy).ToList();
                var startingPoint2 = pointList2[0];

                while (ContinueIntersectionTracing(data2, startingPoint2, pointList2, knotDistance))
                {
                    var point = CalculateNextPoint(data2, pointList2.Last(), knotDistance, epsilon);
                    if (SurfaceConditions.ParametersInBounds(data2, point))
                    {
                        if (pointList2.Count > _intersectionPointsLimit)
                        {
                            throw new InvalidOperationException("Intersection contains too many points, choose different parameters");
                        }
                        if(PointsCloseEnough(data2, point, _pointDistanceEpsilon) == false)
                        {
                            throw new InvalidOperationException("Intersection error, calculated points too far away, choose different parameters");
                        }

                        pointList2.Add(point);
                    }
                    else
                    {
                        pointList2.Add(BoundaryFinder.FindBoundaryPoint(data2, pointList2.Last(), point));
                        break;
                    }
                }

                pointList2.Reverse(); //not necessary, but it works, TODO: test without reverse
                pointList2 = pointList2.Select(x => x.Zwxy).ToList();
                return pointList2;
            }

            return pointList;
        }

        private static bool ContinueIntersectionTracing(IntersectionData data, Vector4 startingPoint, IList<Vector4> pointList, float knotDistance)
        {
            if(pointList.Count > 2)
            {
                var looped = IsLooped(data, startingPoint, pointList[1], pointList.Last(), knotDistance);
                if(looped)
                {
                    pointList.RemoveAt(pointList.Count - 1);
                    pointList.Add(startingPoint);
                    return false;
                }
            }

            return SurfaceConditions.ParametersInBounds(data, pointList.Last());
        }

        private static bool IsLooped(IntersectionData data, Vector4 startingPoint, Vector4 secondPoint, Vector4 currentPoint, float knotDistance, float epsilon = 0.5f)
        {
            var eps = knotDistance * epsilon;
            var stepFirst = StepDistance(data, currentPoint, startingPoint, knotDistance, false, Vector4.Zero);//TODO: check
            var stepSecond = StepDistance(data, currentPoint, secondPoint, knotDistance , false, Vector4.Zero);//TODO: check
            var distCur = (currentPoint - startingPoint).Length;
            var distSec = (secondPoint - startingPoint).Length;
            return (stepFirst >= -knotDistance - eps && stepFirst <= 0.0f + eps && stepSecond <= -knotDistance + eps && distCur <= distSec);
        }

        private static Vector4 CalculateNextPoint(IntersectionData data, Vector4 x0, float knotDistance, float epsilon)
        {
            var maxIterations = 100;
            var xCur = x0;
            var xPrev = x0;

            int i = 0;
            bool useCurrentTangent = false;
            Vector4 xTg = Vector4.Zero;
            while(NewtonCondition(data, xPrev, xCur, x0, knotDistance, useCurrentTangent, xTg, epsilon) && i < maxIterations)
            {
                xPrev = xCur;
                xCur = NewtonIteration(data, xCur, x0, knotDistance, useCurrentTangent, xTg);
                if(IsSharpC0(data, xCur, x0))
                {
                    useCurrentTangent = true;
                    xTg = xCur;
                }
                xCur = ParameterHelper.CorrectUVST(data.p, data.q, xCur);
                i++;
            }

            return xCur; //TODO: throw exception when no solution was found
        }

        private static bool IsSharpC0(IntersectionData data, Vector4 xCur, Vector4 x0)
        {
            if (data.p.DifferentPatches(xCur.X, xCur.Y, x0.X, x0.Y) || data.q.DifferentPatches(xCur.Z, xCur.W, x0.Z, x0.W))
            {
                var tg1 = ICTangent(data, x0);
                var tg2 = ICTangent(data, xCur);
                return Vector3.CalculateAngle(tg1, tg2) > _c0AngleThreshold;
            }
            return false;
        }

        private static bool NewtonCondition(IntersectionData data, Vector4 xPrev, Vector4 xCur, Vector4 x0, float knotDistance, bool useCurrentTangent, Vector4 xTg, float epsilon)
        {
            var canGetValue = SurfaceConditions.ParametersInBounds(data, xCur);
            bool distanceNotOK = true;
            if (canGetValue)
            {
                var pointDiff = data.p.GetVertex(xCur.X, xCur.Y) - data.q.GetVertex(xCur.Z, xCur.W);
                distanceNotOK = pointDiff.Length > epsilon;
            }
            return SurfaceConditions.ParametersInBounds(data, xCur) && //TODO : check condition
                 (distanceNotOK || 
                 Math.Abs(StepDistance(data, xCur, x0, knotDistance, useCurrentTangent, xTg)) > epsilon); //TODO: tweak end condition, use previous approx.
        }

        private static bool PointsCloseEnough(IntersectionData data, Vector4 point, float epsilon)
        {
            var pointDiff = data.p.GetVertex(point.X, point.Y) - data.q.GetVertex(point.Z, point.W);
            return pointDiff.LengthSquared <= epsilon;
        }


        private static Vector4 NewtonIteration(IntersectionData data, Vector4 xCur, Vector4 x0, float knotDistance, bool useCurrentTangent, Vector4 xTg)
        {
            var mat = NewtonMatrix(data, xCur, useCurrentTangent ? xTg : x0);
            var f = F(data, xCur, x0, knotDistance, useCurrentTangent, xTg);
            var dx = LinearEquationSolver.Solve(mat, -f);

            return dx + xCur;
        }

        private static Vector4 F(IntersectionData data, Vector4 xCur, Vector4 x0, float knotDistance, bool useCurrentTangent, Vector4 xTg)
        {
            var p = data.p;
            var q = data.q;

            var u = xCur.X;
            var v = xCur.Y;
            var s = xCur.Z;
            var t = xCur.W;

            var posDiff = p.GetVertex(u, v) - q.GetVertex(s, t);
            var dir = StepDistance(data, xCur, x0, knotDistance, useCurrentTangent, xTg);

            return new Vector4(posDiff, dir);
        }

        private static float StepDistance(IntersectionData data, Vector4 xCur, Vector4 x0, float knotDistance, bool useCurTangent, Vector4 xTg)
        {
            var p = data.p;

            var u = xCur.X;
            var v = xCur.Y;

            var u0 = x0.X;
            var v0 = x0.Y;

            return Vector3.Dot(p.GetVertex(u, v) - p.GetVertex(u0, v0), ICTangent(data, useCurTangent?xTg:x0)) - knotDistance;
        }

        private static Vector3 ICTangent(IntersectionData data, Vector4 x)
        {
            return Vector3.Cross(data.p.Normal(x.X, x.Y), data.q.Normal(x.Z, x.W)).Normalized();
        }


        /// <summary>
        /// x0 may be changed to (first) iteration of x that goes to next patch that turns in a sharp angle to previous patch (C0)
        /// </summary>
        /// <param name="data"></param>
        /// <param name="xCur"></param>
        /// <param name="x0"></param>
        /// <returns></returns>
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
