using avoCADo.Serialization;
using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class NodeImporter
    {
        private NodeFactory _nodeFactory;

        public void Initialize(NodeFactory nodeFactory)
        {
            _nodeFactory = nodeFactory;
        }


        public INode CreateNodeFromItem(NamedType item, Scene scene)
        {
            switch(item)
            {
                case SceneTorus torus:
                    {
                        var createdNode = _nodeFactory.CreateObject(ObjectType.Torus, null);
                        var generator = createdNode.Renderer.GetGenerator() as TorusGenerator;
                        generator.XDivisions = int.Parse(torus.HorizontalSlices);
                        generator.YDivisions = int.Parse(torus.VerticalSlices);
                        var surf = generator.Surface as TorusSurface;
                        surf.MainRadius = (float)(torus.MajorRadius);
                        surf.TubeRadius = (float)(torus.MinorRadius);
                        createdNode.Name = torus.Name;
                        createdNode.Transform.WorldPosition = new OpenTK.Vector3((float)(torus.Position.X), (float)(torus.Position.Y), (float)(torus.Position.Z));
                        createdNode.Transform.RotationEulerAngles = new OpenTK.Vector3((float)(torus.Rotation.X), (float)(torus.Rotation.Y), (float)(torus.Rotation.Z));
                        createdNode.Transform.Scale = new OpenTK.Vector3((float)(torus.Scale.X), (float)(torus.Scale.Y), (float)(torus.Scale.Z));
                        return createdNode;
                    }
                case ScenePoint point:
                    {
                        var createdNode = _nodeFactory.CreateObject(ObjectType.Point, null);
                        createdNode.Name = point.Name;
                        createdNode.Transform.WorldPosition = new OpenTK.Vector3((float)(point.Position.X), (float)(point.Position.Y), (float)(point.Position.Z));
                        return createdNode;
                    }
                case SceneBezierC0 bc0:
                    {
                        var cps = new List<INode>();
                        foreach (var reference in bc0.Points)
                        {
                            var node = FindNodeInScene(reference.Name, scene);
                            if (node != null) cps.Add(node);
                            else throw new InvalidDataException("Referenced node does not exist");
                        }

                        var createdNode = _nodeFactory.CreateObject(ObjectType.BezierCurveC0, new CurveParameters(cps));
                        createdNode.Name = bc0.Name;
                        var generator = createdNode.Renderer.GetGenerator() as BezierGeneratorGeometry;
                        generator.ShowEdges = bc0.ShowControlPolygon;

                        return createdNode;
                    }
                case SceneBezierC2 bc2:
                    {
                        var cps = new List<INode>();
                        foreach (var reference in bc2.Points)
                        {
                            var node = FindNodeInScene(reference.Name, scene);
                            if (node != null) cps.Add(node);
                            else throw new InvalidDataException("Referenced node does not exist");
                        }

                        var createdNode = _nodeFactory.CreateObject(ObjectType.BezierCurveC2, new CurveParameters(cps));
                        createdNode.Name = bc2.Name;
                        var generator = createdNode.Renderer.GetGenerator() as BezierGeneratorGeometry;
                        generator.ShowEdges = bc2.ShowBernsteinPolygon || bc2.ShowDeBoorPolygon; //TODO: divide de boor polygon representation
                        generator.ShowVirtualControlPoints = bc2.ShowBernsteinPoints;
                        return createdNode;
                    }
                case SceneBezierInter ic:
                    {
                        var cps = new List<INode>();
                        foreach (var reference in ic.Points)
                        {
                            var node = FindNodeInScene(reference.Name, scene);
                            if (node != null) cps.Add(node);
                            else throw new InvalidDataException("Referenced node does not exist");
                        }

                        var createdNode = _nodeFactory.CreateObject(ObjectType.InterpolatingCurve, new CurveParameters(cps));
                        createdNode.Name = ic.Name;
                        var generator = createdNode.Renderer.GetGenerator() as BezierGeneratorGeometry;
                        generator.ShowEdges = ic.ShowControlPolygon;
                        return createdNode;
                    }
                case ScenePatchC0 pc0:
                    {
                        var mode = pc0.WrapDirection == WrapType.None ? WrapMode.None : (pc0.WrapDirection == WrapType.Column ? WrapMode.Column : WrapMode.Row);

                        //Calculate patch dimensions
                        var maxRowIdx = 0;
                        var maxColumnIdx = 0;
                        foreach (var point in pc0.Points)
                        {
                            var row = int.Parse(point.Row);
                            var column = int.Parse(point.Column);
                            if (row > maxRowIdx) maxRowIdx = row;
                            if (column > maxColumnIdx) maxColumnIdx = column;
                        }
                        
                        //Fill initial point data
                        var cps = new CoordList<INode>();
                        cps.ResetSize(maxColumnIdx + 1, maxRowIdx + 1);
                        foreach(var point in pc0.Points)
                        {
                            var column = int.Parse(point.Column);
                            var row = int.Parse(point.Row);
                            var node = FindNodeInScene(point.Name, scene);
                            if (node != null) cps[column, row] = node;
                            else throw new InvalidDataException("Referenced node does not exist");
                        }

                        var dims = GetPatchDimensionsFromRowColumn(ObjectType.BezierPatchC0, mode, maxRowIdx + 1, maxColumnIdx + 1); //+1 for count, (n is last index)

                        var createdNode = _nodeFactory.CreateObject(ObjectType.BezierPatchC0,
                            new PatchParameters(mode, dims.horizontalPatches, dims.verticalPatches, 0.0f, 0.0f, cps));
                        createdNode.Name = pc0.Name;
                        var generator = createdNode.Renderer.GetGenerator() as BezierPatchGenerator;
                        generator.ShowEdges = pc0.ShowControlPolygon;
                        generator.IsolineDivisionsU = int.Parse(pc0.ColumnSlices);
                        generator.IsolineDivisionsV = int.Parse(pc0.RowSlices);
                        return createdNode;
                    }
                case ScenePatchC2 pc2:
                    {
                        var mode = pc2.WrapDirection == WrapType.None ? WrapMode.None : (pc2.WrapDirection == WrapType.Column ? WrapMode.Column : WrapMode.Row);
                        
                        //Calculate patch dimensions
                        var maxRowIdx = 0;
                        var maxColumnIdx = 0;
                        foreach (var point in pc2.Points)
                        {
                            var row = int.Parse(point.Row);
                            var column = int.Parse(point.Column);
                            if (row > maxRowIdx) maxRowIdx = row;
                            if (column > maxColumnIdx) maxColumnIdx = column;
                        }

                        //Fill initial point data
                        var cps = new CoordList<INode>();
                        cps.ResetSize(maxColumnIdx + 1, maxRowIdx + 1);
                        foreach (var point in pc2.Points)
                        {
                            var column = int.Parse(point.Column);
                            var row = int.Parse(point.Row);
                            var node = FindNodeInScene(point.Name, scene);
                            if (node != null) cps[column, row] = node;
                            else throw new InvalidDataException("Referenced node does not exist");
                        }

                        var dims = GetPatchDimensionsFromRowColumn(ObjectType.BezierPatchC2, mode, maxRowIdx + 1, maxColumnIdx + 1); //+1 for count, (n is last index)

                        var createdNode = _nodeFactory.CreateObject(ObjectType.BezierPatchC2,
                            new PatchParameters(mode, dims.horizontalPatches, dims.verticalPatches, 0.0f, 0.0f, cps));
                        createdNode.Name = pc2.Name;
                        var generator = createdNode.Renderer.GetGenerator() as BezierPatchGenerator;
                        generator.ShowEdges = pc2.ShowControlPolygon;
                        generator.IsolineDivisionsU = int.Parse(pc2.ColumnSlices);
                        generator.IsolineDivisionsV = int.Parse(pc2.RowSlices);
                        return createdNode;
                    }
                default:
                    return null;
            }
        }

        private (int horizontalPatches, int verticalPatches) GetPatchDimensionsFromRowColumn(ObjectType type, WrapMode mode, int rows, int columns)
        {
            int hor, ver;
            if (type == ObjectType.BezierPatchC0)
            {
                hor = mode == WrapMode.Column ? columns / 3 : (columns - 1) / 3;
                ver = mode == WrapMode.Row ? rows / 3 : (rows - 1) / 3;
            }
            else if (type == ObjectType.BezierPatchC2)
            {
                hor = mode == WrapMode.Column ? columns : columns - 3;
                ver = mode == WrapMode.Row ? rows : rows - 3;
            }
            else throw new InvalidOperationException("Invalid ObjectType. BezierPatchC0/C2 was expected.");
            return (hor, ver);
        }

        private INode FindNodeInScene(string name, Scene scene)
        {
            foreach(var node in scene.Children)
            {
                if (node.Name == name) return node;
            }
            return null;
        }
    }
}
