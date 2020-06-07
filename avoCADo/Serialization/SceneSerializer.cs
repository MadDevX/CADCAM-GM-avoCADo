using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace avoCADo.Serialization
{
    public class SceneSerializer
    {
        public static void SaveSceneTo(string filename, Serialization.Scene sceneToSerialize)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Serialization.Scene));

            using(TextWriter writer = new StreamWriter(filename))
            {
                serializer.Serialize(writer, sceneToSerialize);
            }
        }

        public static Serialization.Scene Serialize(NodeExporter nodeExporter, avoCADo.Scene currentScene)
        {
            var items = new NamedType[currentScene.Children.Count];
           
            for(int i = 0; i < currentScene.Children.Count; i++)
            {
                items[i] = nodeExporter.ExportNode(currentScene.Children[i]);
            }
            var serializedScene = new Serialization.Scene
            {
                Items = items
            };
            return serializedScene;
        }
    }
}
