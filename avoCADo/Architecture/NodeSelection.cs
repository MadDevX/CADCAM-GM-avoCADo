using avoCADo.Architecture;
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
        public static ISelectionManager Manager { get; }
        public static DependencyAddersManager DependencyAddersManager { get; }
        /// <summary>
        /// Used to filter events invocation to reduce overhead
        /// </summary>
        public static readonly int SensibleSelectionLimit = 100;

        static NodeSelection()
        {
            Manager = new SelectionManager();
            DependencyAddersManager = new DependencyAddersManager(Manager);
        }
        
        public static float GetSelectionThreshold(ObjectType type)
        {
            return _distances[type];
        }

        private static Dictionary<ObjectType, float> _distances = DictionaryInitializer.InitializeEnumDictionary<ObjectType, float>
            (
                0.07f,//Point,
                0.3f,//Torus,
                0.3f,//InterpolatingCurve,
                0.3f,//BezierCurveC0,
                0.3f,//BezierCurveC2,
                0.3f,//BezierPatchC0,
                0.3f,//BezierPatchC2,
                0.07f,//VirtualPoint,
                0.0f,//Scene
                0.3f//GregoryPatch
            );
    }
}
