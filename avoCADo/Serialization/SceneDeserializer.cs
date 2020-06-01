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

        struct GroupNodePair
        {
            public INode groupNode;
            public NamedType serializedNode;

            public GroupNodePair(INode groupNode, NamedType serializedNode)
            {
                this.groupNode = groupNode;
                this.serializedNode = serializedNode;
            }
        }

        public static void ImportScene(Serialization.Scene serializedScene, NodeImporter nodeImporter, avoCADo.Scene currentScene)
        {
            var groupNodes = new List<GroupNodePair>();
            foreach(var item in serializedScene.Items)
            {
                var node = nodeImporter.CreateNodeFromItem(item);
                if (node == null) continue;
                if(node.GroupNodeType != GroupNodeType.None)
                {
                    groupNodes.Add(new GroupNodePair(node, item));
                }
            }
        }
    }
}
