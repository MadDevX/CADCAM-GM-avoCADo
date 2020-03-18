using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public static class NameGenerator
    {
        public static string GenerateName(Scene scene, string genericName)
        {
            bool found = false;
            foreach(var node in scene.Nodes)
            {
                if(node.Name.Equals(genericName))
                {
                    found = true;
                    break;
                }
            }
            if(found)
            {
                string indexedName;
                int i = 2;
                do
                {
                    found = false;
                    indexedName = $"{genericName} ({i})";
                    foreach (var node in scene.Nodes)
                    {
                        if (node.Name.Equals(indexedName))
                        {
                            found = true;
                            i++;
                            break;
                        }
                    }
                } while(found);

                return indexedName;
            }
            else
            {
                return genericName;
            }
        }

    }
}
