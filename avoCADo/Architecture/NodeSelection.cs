using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    /// <summary>
    /// Existence of this singleton is explained only by dynamically created WPF controls, 
    /// which would otherwise require handling code-behind, or data-binding definitions.
    /// </summary>
    public static class NodeSelection
    {
        public static SelectionManager Manager { get; } = new SelectionManager();
        /// <summary>
        /// Used to filter events invocation to reduce overhead
        /// </summary>
        public static readonly int SensibleSelectionLimit = 1000;
    }
}
