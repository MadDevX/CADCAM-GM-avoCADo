using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class SceneManager : IDisposable
    {
        private readonly Hierarchy _hierarchy;
        private readonly IInstructionBuffer _instructionBuffer;

        public Scene CurrentScene { get; private set; } = null;
        public SceneManager(Hierarchy hierarchy, IInstructionBuffer instructionBuffer, Scene scene)
        {
            _hierarchy = hierarchy;
            _instructionBuffer = instructionBuffer;
            SetScene(scene);
        }

        public void Dispose()
        {
            if (CurrentScene != null) CurrentScene.Dispose();
        }

        /// <summary>
        /// Returns previous scene (that should be disposed)
        /// </summary>
        /// <param name="scene"></param>
        /// <returns></returns>
        public Scene SetScene(Scene scene)
        {
            var curScene = CurrentScene;
            _hierarchy.Initialize(scene);
            CurrentScene = scene;
            _instructionBuffer.Clear();

            NameGenerator.ResetState();
            NameGenerator.GenerateKeywords(CurrentScene);

            return curScene;
        }

        /// <summary>
        /// Returns previous scene (that should be disposed)
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Scene CreateAndSet(string name)
        {
            var newScene = new Scene(name);
            return SetScene(newScene);
        }
    }
}
