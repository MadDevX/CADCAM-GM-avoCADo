using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
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

        public static void ResetState()
        {
            _objectIds.Clear();
        }

        public static void GenerateKeywords(INode root)
        {
            var names = root.Children.Select((x) => x.Name).ToList();
            names.Sort();
            int i = 0;
            while (i < names.Count)
            {
                var keyword = KeywordWithoutSuffix(names[i]);
                var startIdx = i;
                while (i < names.Count && names[i].StartsWith(keyword)) i++;
                var keywordCount = i - startIdx;
                var lastIdx = FindIndexOf(names[i - 1]);
                _objectIds.Add(keyword, Math.Max(keywordCount, lastIdx+1));
            }
        }   

        private static string KeywordWithoutSuffix(string name)
        {
            var match = Regex.Match(name, "\\s{1}[(]{1}[0-9]+[)]{1}$");
            if(match.Success)
            {
                return name.Substring(0, name.Length - match.Value.Length);
            }
            else
            {
                return name;
            }
        }

        private static int FindIndexOf(string name)
        {
            int i = name.Length - 1;
            while (i > 0 && char.IsDigit(name[i]) == false) i--;
            int numberLastIdx = i;
            while (i > 0 && char.IsDigit(name[i])) i--;
            int numberFirstIdx = i + 1;
            var numberLength = (numberLastIdx + 1) - numberFirstIdx;
            if(int.TryParse(name.Substring(numberFirstIdx, numberLength), out int index))
            {
                return index;
            }
            else
            {
                return 1;
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
