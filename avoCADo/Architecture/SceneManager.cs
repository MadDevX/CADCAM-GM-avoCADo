using avoCADo.Actions;
using avoCADo.Serialization;
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
        private readonly NodeImporter _nodeImporter;

        public Scene CurrentScene { get; private set; } = null;
        public SceneManager(Hierarchy hierarchy, IInstructionBuffer instructionBuffer, NodeImporter nodeImporter, Scene scene)
        {
            _hierarchy = hierarchy;
            _instructionBuffer = instructionBuffer;
            _nodeImporter = nodeImporter;
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
            _instructionBuffer.IssueInstruction<SelectionChangedInstruction, SelectionChangedInstruction.Parameters>(
            new SelectionChangedInstruction.Parameters(null, SelectionChangedInstruction.OperationType.Reset));

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

        public Scene ImportScene(string path)
        {
            var result = SceneDeserializer.Deserialize(path);
            var prevScene = CreateAndSet(NameGenerator.DiscardPath(path, discardExtension: true));
            SceneDeserializer.ImportScene(result, _nodeImporter, CurrentScene);
            NameGenerator.ResetState();
            NameGenerator.GenerateKeywords(CurrentScene);
            return prevScene;
        }
    }
}
