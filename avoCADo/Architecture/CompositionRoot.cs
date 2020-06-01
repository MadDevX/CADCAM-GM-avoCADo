using avoCADo.Architecture;
using avoCADo.HUD;
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
        private NodeFactory _nodeFactory;
        private NodeImporter _nodeImporter;

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

        private SceneManager _sceneManager;

        private Camera _camera;
        private CameraMovement _camMovement;
        private Grid _grid;
        private Cursor3D _cursor;

        private DependencyAddersManager _dependencyAddersManager;
        private TransformationsManager _transformationsManager;
        private CameraModeManager _cameraModeManager;
        private TransformHandler _transformHandler;
        private TorusGeneratorHandler _torusHandler;
        private LabelBindingRefresher _labelBindingRefresher;
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

            _shaderBackgroundManager = new ShaderBackgroundManager(_backgroundManager, _shaderProvider.DefaultShader, _shaderProvider.CurveShader, _shaderProvider.SurfaceShaderBezier);
            _quadRenderer = new QuadOverlayRenderer(_shaderProvider.BufferShader);

            _sceneManager = new SceneManager(_window.hierarchy, new Scene("Main"));
            _camera = new StereoscopicCamera(_viewportManager);
            _camMovement = new CameraMovement(_camera, _control);
            _renderLoop = new RenderLoop(_control, _screenBufferManager, _sceneManager, _camera, _framebufferManager, _quadRenderer, _shaderProvider);

            _screenSelectionManager = new ScreenSelectionHandler(_control, _camera, _sceneManager);
            _rectangularSelectionDrawer = new RectangularSelectionDrawer(_renderLoop, _screenSelectionManager, _shaderProvider.OverlayShader, _control);

            _grid = new Grid(_camera, _renderLoop, new LineRenderer(_shaderProvider.DefaultShader, new GridGenerator(200, 1, _camera)));
            _cursor = new Cursor3D(_control, _shaderProvider.DefaultShader, _renderLoop, _camera);
            
            _dependencyAddersManager = new DependencyAddersManager();
            _transformationsManager = new TransformationsManager(_cursor, _control, _camera, _dependencyAddersManager);

            _cameraModeManager = new CameraModeManager((StereoscopicCamera)_camera, _backgroundManager, backgroundColorStandard, backgroundColorStereoscopic);
            _transformHandler = new TransformHandler(_window.transformView, _window);
            _torusHandler = new TorusGeneratorHandler(_window.torusGeneratorView);
            _labelBindingRefresher = new LabelBindingRefresher(_window, _window.cursor3dInfo, _window.transformationsInfo);
            _nodeFactory = new NodeFactory(_sceneManager, _cursor, _window, _shaderProvider);
            _virtualNodeFactory = new VirtualNodeFactory(_shaderProvider.DefaultShader, _sceneManager);
            Registry.VirtualNodeFactory = _virtualNodeFactory;
            _nodeImporter = new NodeImporter(_nodeFactory);


            _window.cursor3dInfo.DataContext = _cursor;
            _window.transformationsInfo.DataContext = _transformationsManager;
            _window.cameraSettings.DataContext = _cameraModeManager;

            _window.Initialize(_nodeFactory, _transformationsManager, _nodeImporter, _sceneManager);
            TestSceneInitializer.SpawnTestObjects(_sceneManager.CurrentScene, _nodeFactory, _window, _shaderProvider);
        }

        public void Dispose()
        {
            _nodeFactory.Dispose();
            _labelBindingRefresher.Dispose();
            _torusHandler.Dispose();
            _transformHandler.Dispose();
            _transformationsManager.Dispose();
            _dependencyAddersManager.Dispose();
            _cursor.Dispose();
            _grid.Dispose();
            _rectangularSelectionDrawer.Dispose();
            _screenSelectionManager.Dispose();
            _renderLoop.Dispose();
            _camMovement.Dispose();
            _camera.Dispose();
            _sceneManager.Dispose();
            _quadRenderer.Dispose();
            _shaderBackgroundManager.Dispose();
            _shaderProvider.Dispose();
            _framebufferManager.Dispose();
            _viewportManager.Dispose();
            MeshUtility.Dispose();
        }
    }
}
