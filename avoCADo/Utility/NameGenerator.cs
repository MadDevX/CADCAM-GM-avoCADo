using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public static class NameGenerator
    {
        private static Dictionary<string, int> _objectIds = new Dictionary<string, int>();

        public static string GenerateName(INode root, string genericName)
        {
            if(_objectIds.TryGetValue(genericName, out var counter))
            {
                _objectIds[genericName] += 1;
                return genericName + $" ({counter})";
            }
            else
            {
                _objectIds.Add(genericName, 1);
                return genericName;
            }
        }




        /// <summary>
        /// Not usable for creating bezier patches
        /// </summary>
        /// <param name="root"></param>
        /// <param name="genericName"></param>
        /// <returns></returns>
        public static string GenerateNameSlow(INode root, string genericName)
        {
            bool found = false;
            if (root.GroupNodeType != GroupNodeType.None) root = root.Transform.ParentNode;
            foreach(var node in root.Children)
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
                    foreach (var node in root.Children)
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


        public static string DiscardPath(string path, bool discardExtension)
        {
            var splits = path.Split('\\');
            if (splits.Length == 1) splits = path.Split('/');
            var ret = splits[splits.Length - 1];
            if (discardExtension)
            {
                splits = ret.Split('.');
                ret = splits[0];
            }
            return ret;
        }
    }
}
