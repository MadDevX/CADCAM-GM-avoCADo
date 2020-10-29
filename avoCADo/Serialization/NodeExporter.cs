using avoCADo.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class NodeExporter
    {

        public NamedType ExportNode(INode node)
        {
            switch(node.ObjectType)
            {
                case ObjectType.Point:
                    {
                        var pos = node.Transform.WorldPosition;
                        var data = new ScenePoint
                        {
                            Name = node.Name,
                            Position = new ScenePointPosition { X = pos.X, Y = pos.Y, Z = pos.Z }
                        };
                        return data;
                    }
                case ObjectType.Torus:
                    {
                        var pos = node.Transform.WorldPosition;
                        var rot = node.Transform.RotationEulerAngles;
                        var scl = node.Transform.Scale;
                        var gen = node.GetComponent<Renderer>()?.GetGenerator() as TorusGenerator;
                        var surf = gen.Surface as TorusSurface;
                        var data = new SceneTorus
                        {
                            Name = node.Name,
                            Position = new SceneTorusPosition { X = pos.X, Y = pos.Y, Z = pos.Z },
                            Rotation = new SceneTorusRotation { X = rot.X, Y = rot.Y, Z = rot.Z },
                            Scale = new SceneTorusScale { X = scl.X, Y = scl.Y, Z = scl.Z },
                            MinorRadius = surf.TubeRadius,
                            MajorRadius = surf.MainRadius,
                            HorizontalSlices = gen.YDivisions.ToString(),
                            VerticalSlices = gen.XDivisions.ToString()
                        };
                        return data;
                    }
                case ObjectType.BezierCurveC0:
                    {
                        var gen = node.GetComponent<Renderer>()?.GetGenerator() as BezierGeneratorGeometry;
                        var refs = new SceneBezierC0PointRef[node.Children.Count];
                        for(int i = 0; i < node.Children.Count; i++)
                        {
                            refs[i] = new SceneBezierC0PointRef() { Name = node.Children[i].Name };
                        }
                        var data = new SceneBezierC0
                        {
                            Name = node.Name,
                            ShowControlPolygon = gen.ShowEdges,
                            Points = refs
                        };
                        return data;
                    }
                case ObjectType.BezierCurveC2:
                    {
                        var gen = node.GetComponent<Renderer>()?.GetGenerator() as BezierGeneratorGeometry;
                        var refs = new SceneBezierC2PointRef[node.Children.Count];
                        for (int i = 0; i < node.Children.Count; i++)
                        {
                            refs[i] = new SceneBezierC2PointRef() { Name = node.Children[i].Name };
                        }
                        var data = new SceneBezierC2
                        {
                            Name = node.Name,
                            ShowBernsteinPolygon = gen.ShowEdges,
                            ShowDeBoorPolygon = gen.ShowEdges,
                            ShowBernsteinPoints = gen.ShowVirtualControlPoints,
                            Points = refs
                        };
                        return data;
                    }
                case ObjectType.InterpolatingCurve:
                    {
                        var gen = node.GetComponent<Renderer>()?.GetGenerator() as BezierGeneratorGeometry;
                        var refs = new SceneBezierInterPointRef[node.Children.Count];
                        for (int i = 0; i < node.Children.Count; i++)
                        {
                            refs[i] = new SceneBezierInterPointRef() { Name = node.Children[i].Name };
                        }
                        var data = new SceneBezierInter
                        {
                            Name = node.Name,
                            ShowControlPolygon = gen.ShowEdges,
                            Points = refs
                        };
                        return data;
                    }
                case ObjectType.BezierPatchC0:
                    {
                        var gen = node.GetComponent<Renderer>()?.GetGenerator() as BezierPatchGenerator;
                        var refs = new ScenePatchC0PointRef[node.Children.Count];
                        var coordList = gen.Surface.ControlPoints;
                        var refIdx = 0;
                        for(int x = 0; x < coordList.DataWidth; x++)
                        {
                            for(int y = 0; y < coordList.DataHeight; y++)
                            {
                                refs[refIdx++] = new ScenePatchC0PointRef()
                                {
                                    Name = coordList[x, y].Name,
                                    Column = x.ToString(),
                                    Row = y.ToString()
                                };
                            }
                        }
                        var data = new ScenePatchC0()
                        {
                            Name = node.Name,
                            WrapDirection = SerializationUtility.ConvertToWrapType(gen.WrapMode),
                            ShowControlPolygon = gen.ShowEdges,
                            ColumnSlices = gen.IsolineDivisionsU.ToString(),
                            RowSlices = gen.IsolineDivisionsV.ToString(),
                            Points = refs
                        };
                        return data;
                    }
                case ObjectType.BezierPatchC2:
                    {
                        var gen = node.GetComponent<Renderer>()?.GetGenerator() as BezierPatchC2Generator;
                        var refs = new ScenePatchC2PointRef[node.Children.Count];
                        var coordList = gen.Surface.ControlPoints;
                        var refIdx = 0;
                        for (int x = 0; x < coordList.DataWidth; x++)
                        {
                            for (int y = 0; y < coordList.DataHeight; y++)
                            {
                                refs[refIdx++] = new ScenePatchC2PointRef()
                                {
                                    Name = coordList[x, y].Name,
                                    Column = x.ToString(),
                                    Row = y.ToString()
                                };
                            }
                        }
                        var data = new ScenePatchC2()
                        {
                            Name = node.Name,
                            WrapDirection = SerializationUtility.ConvertToWrapType(gen.WrapMode),
                            ShowControlPolygon = gen.ShowEdges,
                            ColumnSlices = gen.IsolineDivisionsU.ToString(),
                            RowSlices = gen.IsolineDivisionsV.ToString(),
                            Points = refs
                        };
                        return data;
                    }
            }
            return null;
        }
    }
}
