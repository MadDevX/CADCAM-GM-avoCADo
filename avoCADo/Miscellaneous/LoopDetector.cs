using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.Miscellaneous
{
    public class LoopDetector
    {
        public static bool AreConnected(INode a, INode b, INode c)
        {
            var interusedA = GetChildrenWithMultipleUniqueDependencies(a);
            var interusedB = GetChildrenWithMultipleUniqueDependencies(b);
            var interusedC = GetChildrenWithMultipleUniqueDependencies(c);
            var commonAB = GetCommon(interusedA, interusedB);
            var commonBC = GetCommon(interusedB, interusedC);
            var commonCA = GetCommon(interusedC, interusedA);
            if (commonAB != null && commonBC != null && commonCA != null) return true;
            else return false;
        }


        private static INode GetCommon(IList<INode> a, IList<INode> b)
        {
            foreach (var ch1 in a)
            {
                foreach (var ch2 in b)
                {
                    if (ch1 == ch2) return ch1;
                }
            }
            return null;
        }

        private static IList<INode> GetChildrenWithMultipleUniqueDependencies(INode node)
        {
            var list = new List<INode>();
            foreach(var child in node.Children)
            {
                if((child as IDependencyCollector).UniqueDependencyCount > 1)
                {
                    list.Add(child);
                }
            }
            return list;
        }
    }
}
