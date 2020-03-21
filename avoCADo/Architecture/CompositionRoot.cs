using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class CompositionRoot : IDisposable
    {
        public NodeFactory NodeFactory { get; private set; }

        private MainWindow _window;
        private GLControl _control;

        private ScreenBufferManager _screenBufferManager;
        private ViewportManager _viewportManager;

        private Shader _shader;
        private Scene _scene;
        private Camera _camera;
        private CameraMovement _camMovement;
        private Cursor3D _cursor;

        private TransformHandler _transformHandler;
        private TorusGeneratorHandler _torusHandler;
        private TransformationModeHandler _transformationModeHandler;
        private ScreenSelectionManager _selectionManager;
        private RenderLoop _renderLoop;

        public CompositionRoot(GLControl control, MainWindow window)
        {
            _window = window;
            _control = control;
            Initialize();
        }

        private void Initialize()
        {
            _screenBufferManager = new ScreenBufferManager(Color.FromArgb(255, Color.FromArgb(40, 40, 40)));
            _viewportManager = new ViewportManager(_control);
            _shader = new Shader("vs.vert", "fs.frag");
            _scene = new Scene("Main");
            _camera = new Camera(_viewportManager);
            _camMovement = new CameraMovement(_camera, _control);
            _renderLoop = new RenderLoop(_control, _screenBufferManager, _scene, _camera);

            _selectionManager = new ScreenSelectionManager(_control, _camera, _scene);
            _cursor = new Cursor3D(_control, _window.transformationsLabel, _shader, _renderLoop, _window, _camera);
            _transformHandler = new TransformHandler(_window.transformView, _window);
            _torusHandler = new TorusGeneratorHandler(_window.torusGeneratorView);
            _transformationModeHandler = new TransformationModeHandler(_window, _cursor);
            NodeFactory = new NodeFactory(_scene, _cursor, _shader);

            _window.hierarchy.treeView.Items.Add(_scene);
            TestSceneInitializer.SpawnTestObjects(_scene, _shader);
        }

        public void Dispose()
        {
            _transformationModeHandler.Dispose();
            _torusHandler.Dispose();
            _transformHandler.Dispose();
            _cursor.Dispose();
            _selectionManager.Dispose();
            _renderLoop.Dispose();
            _camMovement.Dispose();
            _camera.Dispose();
            _scene.Dispose();
            _shader.Dispose();
            _viewportManager.Dispose();
        }
    }
}
