using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace avoCADo.Serialization
{
    public class SceneDeserializer
    {
        public static Serialization.Scene Deserialize(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Serialization.Scene));

            Serialization.Scene deserializedScene;

            using (Stream reader = new FileStream(filename, FileMode.Open))
            {
                deserializedScene = (Serialization.Scene)serializer.Deserialize(reader);
            }

            return deserializedScene;
        }

        public static void ImportScene(Serialization.Scene serializedScene, NodeImporter nodeImporter, avoCADo.Scene currentScene)
        {
            var groupNodes = new List<NamedType>();
            foreach(var item in serializedScene.Items)
            {
                if (IsGroupObject(GetObjectType(item)) == false)
                {
                    var node = nodeImporter.CreateNodeFromItem(item, currentScene);
                    if (node == null) throw new InvalidDataException("Scene file is invalid!");
                }
                else
                {
                    groupNodes.Add(item);
                }
            }
            foreach(var item in groupNodes)
            {
                var node = nodeImporter.CreateNodeFromItem(item, currentScene);
                if (node == null) throw new InvalidDataException("Scene file is invalid!");
            }
        }

        private static ObjectType GetObjectType(NamedType namedType)
        {
            if (namedType is SceneTorus) return ObjectType.Torus;
            if (namedType is ScenePoint) return ObjectType.Point;
            if (namedType is SceneBezierC0) return ObjectType.BezierCurveC0;
            if (namedType is SceneBezierC2) return ObjectType.BezierCurveC2;
            if (namedType is SceneBezierInter) return ObjectType.InterpolatingCurve;
            if (namedType is ScenePatchC0) return ObjectType.BezierPatchC0;
            if (namedType is ScenePatchC2) return ObjectType.BezierPatchC2;
            throw new InvalidDataException("Invalid object type");
        }

        private static bool IsGroupObject(ObjectType type)
        {
            switch(type)
            {
                case ObjectType.BezierCurveC0:
                case ObjectType.BezierCurveC2:
                case ObjectType.InterpolatingCurve:
                case ObjectType.BezierPatchC0:
                case ObjectType.BezierPatchC2:
                    return true;
            }
            return false;
        }
    }
}
