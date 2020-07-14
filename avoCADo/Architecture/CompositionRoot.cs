using avoCADo.Architecture;
using avoCADo.HUD;
using avoCADo.Utility;
using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace avoCADo
{
    public class CompositionRoot : IDisposable
    {
        private InstructionBuffer _instructionBuffer;
        private PointNodePool _pointNodePool;
        private NodeFactory _nodeFactory;
        private NodeImporter _nodeImporter;
        private NodeExporter _nodeExporter;

        private MainWindow _window;
        private GLControl _control;

        private BackgroundManager _backgroundManager;
        private ScreenBufferManager _screenBufferManager;
        private ViewportManager _viewportManager;
        private FramebufferManager _framebufferManager;

        private ShaderProvider _shaderProvider;

        private ShaderBackgroundManager _shaderBackgroundManager;
        private QuadOverlayRenderer _quadRenderer;

        private SceneManager _sceneManager;

        private Camera _camera;
        private CameraMovement _camMovement;
        private Grid _grid;
        private Cursor3D _cursor;

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

            _instructionBuffer = new InstructionBuffer();

            _backgroundManager = new BackgroundManager(backgroundColorStandard);
            _viewportManager = new ViewportManager(_control);
            _screenBufferManager = new ScreenBufferManager(_viewportManager, _backgroundManager, _control);
            _framebufferManager = new FramebufferManager(2, _viewportManager, _backgroundManager, _control);

            _shaderProvider = new ShaderProvider();
            _nodeImporter = new NodeImporter();
            _nodeExporter = new NodeExporter();

            _shaderBackgroundManager = new ShaderBackgroundManager(_backgroundManager, _shaderProvider.DefaultShader, _shaderProvider.CurveShader, _shaderProvider.SurfaceShaderBezier, _shaderProvider.SurfaceShaderDeBoor, _shaderProvider.SurfaceShaderGregory);
            _quadRenderer = new QuadOverlayRenderer(_shaderProvider.BufferShader);

            _sceneManager = new SceneManager(_window.hierarchy, _instructionBuffer, _nodeImporter, new Scene("Main"));
            _camera = new StereoscopicCamera(_viewportManager);
            _camMovement = new CameraMovement(_camera, _control);
            _renderLoop = new RenderLoop(_control, _screenBufferManager, _viewportManager, _sceneManager, _camera, _framebufferManager, _quadRenderer, _shaderProvider);

            _screenSelectionManager = new ScreenSelectionHandler(_control, _camera, _sceneManager, _instructionBuffer);
            _rectangularSelectionDrawer = new RectangularSelectionDrawer(_renderLoop, _screenSelectionManager, _shaderProvider.OverlayShader, _control);

            _grid = new Grid(_camera, _renderLoop, new LineRenderer(_shaderProvider.DefaultShader, new GridGenerator(200, 1, _camera)));
            _cursor = new Cursor3D(_control, _shaderProvider.DefaultShader, _renderLoop, _camera);
            
            _transformationsManager = new TransformationsManager(_cursor, _control, _camera, _instructionBuffer);

            _cameraModeManager = new CameraModeManager((StereoscopicCamera)_camera, _backgroundManager, backgroundColorStandard, backgroundColorStereoscopic);
            _transformHandler = new TransformHandler(_window.transformView, _window);
            _torusHandler = new TorusGeneratorHandler(_window.torusGeneratorView);
            _labelBindingRefresher = new LabelBindingRefresher(_window, _window.cursor3dInfo, _window.transformationsInfo);

            _pointNodePool = new PointNodePool(_shaderProvider);
            _nodeFactory = new NodeFactory(_sceneManager, _cursor, _shaderProvider, _pointNodePool);
            _virtualNodeFactory = new VirtualNodeFactory(_shaderProvider.DefaultShader, _sceneManager);
            _nodeImporter.Initialize(_nodeFactory);

            Registry.VirtualNodeFactory = _virtualNodeFactory;
            Registry.InstructionBuffer = _instructionBuffer;
            Registry.NodeFactory = _nodeFactory;
            Registry.ShaderProvider = _shaderProvider;

            _window.transformView.Initialize(_instructionBuffer, _cursor);
            _window.cursor3dInfo.Initialize(_cursor);
            _window.transformationsInfo.DataContext = _transformationsManager;
            _window.cameraSettings.DataContext = _cameraModeManager;

            _window.Initialize(_nodeFactory, _transformationsManager, _nodeImporter, _nodeExporter, _sceneManager, _cursor, _renderLoop, _instructionBuffer);

            _sceneManager.ImportScene("D:\\Studia\\Semestr I Mag\\MG1\\torusIntersectionTestEdgeCase.xml");
            _cursor.Transform.Position = new Vector3(-0.413268f, 0.1434115f, 0.2349933f);
            //TestSceneInitializer.SpawnTestObjects(_sceneManager.CurrentScene, _nodeFactory, _window, _shaderProvider, _cursor);


            ////var bitMap = new Bitmap("inputImage.bmp");
            ////var rgbMap = new Bitmap(bitMap.Width, bitMap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            ////using(var gr = Graphics.FromImage(rgbMap))
            ////{
            ////    gr.DrawImage(bitMap, new Rectangle(0, 0, rgbMap.Width, rgbMap.Height));
            ////}

            ////var result = TrimTextureGenerator.FillBitmap(rgbMap, 15, 15);

            ////rgbMap.Save("convertedImage.bmp");
            ////result.Save("filledImage.bmp");

            ////rgbMap.Dispose();
            ////bitMap.Dispose();
            ////result.Dispose();
        }


        public void Dispose()
        {
            _pointNodePool.Dispose();
            _labelBindingRefresher.Dispose();
            _torusHandler.Dispose();
            _transformHandler.Dispose();
            _transformationsManager.Dispose();
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
            NodeSelection.DependencyAddersManager.Dispose();
        }
    }
}
