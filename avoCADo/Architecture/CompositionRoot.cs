using avoCADo.Architecture;
using avoCADo.Utility;
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

        private BackgroundManager _backgroundManager;
        private ScreenBufferManager _screenBufferManager;
        private ViewportManager _viewportManager;
        private FramebufferManager _framebufferManager;
        private TessellationManager _tesselationManager;

        private ShaderProvider _shaderProvider;

        private ShaderBackgroundManager _shaderBackgroundManager;
        private QuadOverlayRenderer _quadRenderer;

        private Scene _scene;
        private Camera _camera;
        private CameraMovement _camMovement;
        private Grid _grid;
        private Cursor3D _cursor;

        private StereoscopicCameraModeManager _cameraModeManager;
        private TransformHandler _transformHandler;
        private TorusGeneratorHandler _torusHandler;
        private TransformationModeHandler _transformationModeHandler;
        private ScreenSelectionHandler _screenSelectionManager;
        private RectangularSelectionDrawer _rectangularSelectionDrawer;
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
            MeshUtility.Initialize();
            var backgroundColorStereoscopic = new Color4(0.0f, 0.0f, 0.0f, 1.0f);
            var backgroundColorStandard = new Color4(0.157f, 0.157f, 0.157f, 1.0f);

            _backgroundManager = new BackgroundManager(backgroundColorStandard);
            _screenBufferManager = new ScreenBufferManager(_backgroundManager);
            _viewportManager = new ViewportManager(_control);
            _framebufferManager = new FramebufferManager(2, _viewportManager, _backgroundManager);
            _tesselationManager = new TessellationManager();

            _shaderProvider = new ShaderProvider();

            _shaderBackgroundManager = new ShaderBackgroundManager(_backgroundManager, _shaderProvider.DefaultShader, _shaderProvider.CurveShader);
            _quadRenderer = new QuadOverlayRenderer(_shaderProvider.BufferShader);

            _scene = new Scene("Main");
            _camera = new StereoscopicCamera(_viewportManager);
            _camMovement = new CameraMovement(_camera, _control);
            _renderLoop = new RenderLoop(_control, _screenBufferManager, _scene, _camera, _framebufferManager, _quadRenderer, _shaderProvider);

            _screenSelectionManager = new ScreenSelectionHandler(_control, _camera, _scene);
            _rectangularSelectionDrawer = new RectangularSelectionDrawer(_renderLoop, _screenSelectionManager, _shaderProvider.OverlayShader, _control);

            _grid = new Grid(_camera, _renderLoop, new LineRenderer(_shaderProvider.DefaultShader, new GridGenerator(200, 1, _camera)));
            _cursor = new Cursor3D(_control, _window.transformationsLabel, _shaderProvider.DefaultShader, _renderLoop, _window, _camera);

            _cameraModeManager = new StereoscopicCameraModeManager((StereoscopicCamera)_camera, _backgroundManager, backgroundColorStandard, backgroundColorStereoscopic);
            _transformHandler = new TransformHandler(_window.transformView, _window);
            _torusHandler = new TorusGeneratorHandler(_window.torusGeneratorView);
            _transformationModeHandler = new TransformationModeHandler(_window, _cursor);
            NodeFactory = new NodeFactory(_scene, _cursor, _shaderProvider.DefaultShader, _shaderProvider.CurveShader, _shaderProvider.SurfaceShader);
            _virtualNodeFactory = new VirtualNodeFactory(_shaderProvider.DefaultShader, _scene);
            Registry.VirtualNodeFactory = _virtualNodeFactory;

            _window.cameraSettings.DataContext = _cameraModeManager;
            _window.hierarchy.treeView.Items.Add(_scene);
            TestSceneInitializer.SpawnTestObjects(_scene, NodeFactory, _shaderProvider.DefaultShader, _shaderProvider.CurveShader, _shaderProvider.SurfaceShader);
        }

        public void Dispose()
        {
            NodeFactory.Dispose();
            _transformationModeHandler.Dispose();
            _torusHandler.Dispose();
            _transformHandler.Dispose();
            _cursor.Dispose();
            _grid.Dispose();
            _rectangularSelectionDrawer.Dispose();
            _screenSelectionManager.Dispose();
            _renderLoop.Dispose();
            _camMovement.Dispose();
            _camera.Dispose();
            _scene.Dispose();
            _quadRenderer.Dispose();
            _shaderBackgroundManager.Dispose();
            _shaderProvider.Dispose();
            _framebufferManager.Dispose();
            _viewportManager.Dispose();
            MeshUtility.Dispose();
        }
    }
}
