﻿using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.Windows;
using OpenTK.Graphics.OpenGL;

using Size = System.Drawing.Size;
using Color = System.Drawing.Color;
using System.Windows.Threading;
using System.Diagnostics;
using System.Windows.Media;
using avoCADo.HUD;
using avoCADo.Serialization;

namespace avoCADo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IUpdateLoop
    {
        private NodeFactory _nodeFactory;
        private NodeImporter _nodeImporter;
        private TransformationsManager _transformationsManager;
        private SceneManager _sceneManager;

        private GLControl _glControl;
        private CompositionRoot _compositionRoot;
        private DispatcherTimer _timer;
        private Stopwatch _deltaStopwatch;

        public event Action<float> OnUpdateLoop;

        public MainWindow()
        {
            InitializeComponent();

            CreateGLControl();
            _compositionRoot = new CompositionRoot(_glControl, this);

            InitLoop();

            //var v1 = new Vector3(2.0f, 3.0f, 4.0f);
            //var v2 = new Vector3(4.0f, 5.0f, 6.0f);
            //var res = Vector3.Divide(v1, v2);
            //System.Windows.Forms.MessageBox.Show(res.ToString(), "res", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        public void Initialize(NodeFactory nodeFactory, TransformationsManager transformationsManager, NodeImporter nodeImporter, SceneManager sceneManager)
        {
            _nodeFactory = nodeFactory;
            _transformationsManager = transformationsManager;
            _nodeImporter = nodeImporter;
            _sceneManager = sceneManager;
        }

        private void CreateGLControl()
        {
            _glControl = new GLControl(new OpenTK.Graphics.GraphicsMode(32, 24, 8, 8));
            _glControl.Dock = DockStyle.Fill;
            Host.Child = _glControl;
            _glControl.MakeCurrent();
        }

        private void InitLoop()
        {
            _deltaStopwatch = new Stopwatch();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(16);
            _timer.Tick += SetDirty;
            _timer.Start();

            CompositionTarget.Rendering += OnTick;
            _deltaStopwatch.Start();
        }

        private void SetDirty(object sender, EventArgs e)
        {
            Host.InvalidateVisual();
        }

        private void OnTick(object sender, EventArgs e)
        {
            var deltaTime = (float)_deltaStopwatch.Elapsed.TotalSeconds;
            _deltaStopwatch.Restart();
            OnUpdateLoop?.Invoke(deltaTime);
            _glControl.Invalidate();
        }

        protected override void OnClosed(EventArgs e)
        {
            CompositionTarget.Rendering -= OnTick;
            _compositionRoot.Dispose();
            _timer.Tick -= SetDirty;
            _timer.Stop();
            base.OnClosed(e);
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == Key.System && e.OriginalSource is System.Windows.Forms.Integration.WindowsFormsHost)
            {
                e.Handled = true;
            }
        }
    }
}
