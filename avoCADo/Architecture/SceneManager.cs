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
        public Scene CurrentScene { get; private set; } = null;
        public SceneManager(Hierarchy hierarchy, Scene scene)
        {
            _hierarchy = hierarchy;
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
            return curScene;
        }

        /// <summary>
        /// Returns previous scene (that should be disposed)
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Scene CreateNew(string name)
        {
            var newScene = new Scene(name);
            return SetScene(newScene);
        }
    }
}
