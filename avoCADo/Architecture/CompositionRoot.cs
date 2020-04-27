using avoCADo.Architecture;
using OpenTK;
using OpenTK.Graphics;
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

        private ShaderWrapper _defaultShader;
        private ShaderWrapper _curveShader;
        private Scene _scene;
        private Camera _camera;
        private CameraMovement _camMovement;
        private Grid _grid;
        private Cursor3D _cursor;

        private TransformHandler _transformHandler;
        private TorusGeneratorHandler _torusHandler;
        private TransformationModeHandler _transformationModeHandler;
        private ScreenSelectionHandler _screenSelectionManager;
        private RenderLoop _renderLoop;
        private VirtualNodeFactory _virtualNodeFactory;

        public CompositionRoot(GLControl control, MainWindow window)
        {
            _window = window;
            _control = control;
            Initialize();
        }

        private void Initialize()
        {
            var backgroundColor = new Color4(0.157f, 0.157f, 0.157f, 1.0f);
            _screenBufferManager = new ScreenBufferManager(backgroundColor);
            _viewportManager = new ViewportManager(_control);
            _defaultShader = new ShaderWrapper(new Shader("vs.vert", "fs.frag"));
            _defaultShader.SetBackgroundColor(backgroundColor);
            _curveShader = new ShaderWrapper(new Shader("vs.vert", "gsBezierC0.geom", "fs.frag"));
            _curveShader.SetBackgroundColor(backgroundColor);
            _scene = new Scene("Main");
            _camera = new Camera(_viewportManager);
            _camMovement = new CameraMovement(_camera, _control);
            _renderLoop = new RenderLoop(_control, _screenBufferManager, _scene, _camera);

            _screenSelectionManager = new ScreenSelectionHandler(_control, _camera, _scene);

            _grid = new Grid(_camera, _renderLoop, new LineRenderer(_defaultShader, new GridGenerator(200, 1, _camera)));
            _cursor = new Cursor3D(_control, _window.transformationsLabel, _defaultShader, _renderLoop, _window, _camera);
            _transformHandler = new TransformHandler(_window.transformView, _window);
            _torusHandler = new TorusGeneratorHandler(_window.torusGeneratorView);
            _transformationModeHandler = new TransformationModeHandler(_window, _cursor);
            NodeFactory = new NodeFactory(_scene, _cursor, _defaultShader, _curveShader);
            _virtualNodeFactory = new VirtualNodeFactory(_defaultShader, _scene);
            Registry.VirtualNodeFactory = _virtualNodeFactory;

            _window.hierarchy.treeView.Items.Add(_scene);
            TestSceneInitializer.SpawnTestObjects(_scene, _defaultShader, _curveShader);
        }

        public void Dispose()
        {
            _transformationModeHandler.Dispose();
            _torusHandler.Dispose();
            _transformHandler.Dispose();
            _cursor.Dispose();
            _grid.Dispose();
            _screenSelectionManager.Dispose();
            _renderLoop.Dispose();
            _camMovement.Dispose();
            _camera.Dispose();
            _scene.Dispose();
            _curveShader.Dispose();
            _defaultShader.Dispose();
            _viewportManager.Dispose();
        }
    }
}
