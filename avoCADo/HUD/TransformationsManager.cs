using avoCADo.Actions;
using avoCADo.Architecture;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace avoCADo.HUD
{
    public enum TransformationMode
    {
        Local,
        Cursor
    }
    public enum TransformationType
    {
        None,
        Translation,
        Rotation,
        Scale
    }
    public enum SnapMode
    {
        None,
        SnapValue,
        SnapToGrid
    }

    public class TransformationsManager : IDisposable
    {
        private TransformationMode _mode = TransformationMode.Local;
        public TransformationMode Mode 
        {
            get => _mode; 
            set
            {
                _mode = value;
                _instructionUtility.BreakInstructions();
            }
        }
        private TransformationType _transformationType = TransformationType.None;
        public TransformationType TransformationType 
        {
            get => _transformationType;
            private set
            {
                _transformationType = value;
                _instructionUtility.BreakInstructions();
            }
        }


        public float SnapValue { get; set; } = 0.1f;

        private SnapMode _snapToGrid = SnapMode.None;
        public SnapMode SnapMode
        {
            get => _snapToGrid;
            set
            {
                _snapToGrid = value;
                _currentInputBuffer = Point.Empty;
            }
        }
        public string Axis 
        { 
            get
            {
                var s = "";
                if (Mults.X > 0.0f) s += "X";
                if (Mults.Y > 0.0f) s += "Y";
                if (Mults.Z > 0.0f) s += "Z";
                if (s == "") s = "Any/None";
                return s;
            }
        }

        private readonly ISelectionManager _selectionManager;
        private readonly Cursor3D _cursor3D;
        private readonly GLControl _control;
        private readonly Camera _camera;
        private readonly DependencyAddersManager _dependencyAddersManager;
        private readonly InstructionBuffer _instructionBuffer;
        private TransformationInstructionUtility _instructionUtility;
        private float TranslateMultiplier => (_translateSensitivity / _control.Width) * _camera.DistanceToTarget;
        private float RotateMultiplier => (_rotateSensitivity / _control.Width);
        private float ScaleMultiplier => (_scaleSensitivity / _control.Width);

        private Point _prevPos;
        private float _rotateSensitivity = (float)Math.PI * 1.0f;//5.0f;
        private float _translateSensitivity = 2.5f;
        private float _scaleSensitivity = 5.0f;

        private Vector3 _mults = Vector3.Zero;
        private Vector3 Mults
        {
            get => _mults;
            set
            {
                _mults = value;
                _instructionUtility.BreakInstructions();
            }
        }


        private Point _currentInputBuffer = Point.Empty;

        public TransformationsManager(Cursor3D cursor3D, GLControl control, Camera camera, InstructionBuffer instructionBuffer)
        {
            _selectionManager = NodeSelection.Manager;
            _dependencyAddersManager = NodeSelection.DependencyAddersManager;
            _cursor3D = cursor3D;
            _control = control;
            _camera = camera;
            _instructionBuffer = instructionBuffer;
            _instructionUtility = new TransformationInstructionUtility(_instructionBuffer, _cursor3D);
            Initialize();
        }

        private void Initialize()
        {
            _control.KeyDown += KeyDown;
            _control.MouseMove += MouseMove;
        }

        public void Dispose()
        {
            _control.KeyDown -= KeyDown;
            _control.MouseMove -= MouseMove;
        }

        private void KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (_selectionManager.MainSelection != null)
            {
                if (e.KeyCode == System.Windows.Forms.Keys.T) TransformationType = TransformationType.Translation;
                else if (e.KeyCode == System.Windows.Forms.Keys.R) TransformationType = TransformationType.Rotation;
                else if (e.KeyCode == System.Windows.Forms.Keys.S) TransformationType = TransformationType.Scale;
                else if (e.KeyCode == System.Windows.Forms.Keys.Escape) 
                {
                    TransformationType = TransformationType.None; 
                    Mults = Vector3.Zero;
                }
            }
            else
            {
                TransformationType = TransformationType.None;
                Mults = Vector3.Zero;
            }
            if (TransformationType != TransformationType.None)
            {
                if (TransformationType == TransformationType.Scale)
                {
                    if      (e.KeyCode == System.Windows.Forms.Keys.X) Mults = new Vector3(1.0f, Mults.Y, Mults.Z);
                    else if (e.KeyCode == System.Windows.Forms.Keys.Y) Mults = new Vector3(Mults.X, 1.0f, Mults.Z);
                    else if (e.KeyCode == System.Windows.Forms.Keys.Z) Mults = new Vector3(Mults.X, Mults.Y, 1.0f);
                }
                else
                {
                    if      (e.KeyCode == System.Windows.Forms.Keys.X) Mults = Vector3.UnitX;
                    else if (e.KeyCode == System.Windows.Forms.Keys.Y) Mults = Vector3.UnitY;
                    else if (e.KeyCode == System.Windows.Forms.Keys.Z) Mults = Vector3.UnitZ;
                }
            }
        }

        private void MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Point posDiff = new Point(0, 0);

            var notifyDependencyAdders = false;

            if (TransformationType != TransformationType.None)
            {
                posDiff = new Point(e.Location.X - _prevPos.X, e.Location.Y - _prevPos.Y);
                if (Math.Abs(posDiff.X) > 100 || Math.Abs(posDiff.Y) > 100)
                {
                    posDiff = Point.Empty;
                }
                if (_selectionManager.MainSelection == null)
                {
                    TransformationType = TransformationType.None;
                    Mults = Vector3.Zero;
                }
                notifyDependencyAdders = true;
            }

            Vector3 diffVector;
            switch (TransformationType)
            {
                case TransformationType.Translation:
                    posDiff = BufferInput(posDiff, TranslateMultiplier);
                    diffVector = new Vector3(Mults.X * posDiff.X, Mults.Y * posDiff.X, Mults.Z * posDiff.X); //only left-right  mouse movement is used as transformation input
                    HandleTranslation(diffVector, posDiff);
                    break;
                case TransformationType.Rotation:
                    posDiff = BufferInput(posDiff, RotateMultiplier);
                    diffVector = new Vector3(Mults.X * posDiff.X, Mults.Y * posDiff.X, Mults.Z * posDiff.X); //only left-right  mouse movement is used as transformation input
                    HandleRotation(diffVector);
                    break;
                case TransformationType.Scale:
                    posDiff = BufferInput(posDiff, ScaleMultiplier);
                    diffVector = new Vector3(Mults.X * posDiff.X, Mults.Y * posDiff.X, Mults.Z * posDiff.X); //only left-right  mouse movement is used as transformation input
                    HandleScale(diffVector, posDiff);
                    break;
            }

            _prevPos = e.Location;

            if(notifyDependencyAdders)
            {
                _dependencyAddersManager.NotifyDependencyAdders();
            }
        }

        #region Transformations Processing
        private Point BufferInput(Point posDiff, float TransformationMultiplier)
        {
            if (SnapMode != SnapMode.None)
            {
                var snapDiv = (int)(SnapValue / TransformationMultiplier);
                _currentInputBuffer.X += posDiff.X;
                _currentInputBuffer.Y += posDiff.Y;
                Point snappedPosDiff = Point.Empty;
                if (Math.Abs(_currentInputBuffer.X) >= snapDiv)
                {
                    snappedPosDiff.X = snapDiv * Math.Sign(_currentInputBuffer.X);
                    _currentInputBuffer.X = 0;
                }
                if (Math.Abs(_currentInputBuffer.Y) >= snapDiv)
                {
                    snappedPosDiff.Y = snapDiv * Math.Sign(_currentInputBuffer.Y);
                    _currentInputBuffer.Y = 0;
                }
                return snappedPosDiff;
            }
            else return posDiff;
        }

        private void HandleTranslation(Vector3 diffVector, Point posDiff)
        {
            if (Mults.Length > 0.0f)
            {
                diffVector *= TranslateMultiplier;
            }
            else
            {
                diffVector = _camera.ViewPlaneVectorToWorldSpace(new Vector2(posDiff.X, -posDiff.Y)) * TranslateMultiplier;
            }

            if(SnapMode == SnapMode.SnapValue)
            {
                diffVector = diffVector.RoundToDivisionValue(SnapValue);
            }
            
            _instructionUtility.UpdateInstruction(Mode, TransformationType);
            TranslateRaw(_selectionManager.SelectedNodes, diffVector, Mode, _cursor3D.Position);
        }

        private void HandleRotation(Vector3 diffVector)
        {
            diffVector *= RotateMultiplier;
            if (SnapMode == SnapMode.SnapValue)
            {
                diffVector = diffVector.RoundToDivisionValue(SnapValue, (float)Math.PI*0.5f);
            }

            _instructionUtility.UpdateInstruction(Mode, TransformationType);
            RotateRaw(_selectionManager.SelectedNodes, diffVector, Mode, _cursor3D.Position);
        }

        private void HandleScale(Vector3 diffVector, Point posDiff)
        {
            diffVector *= ScaleMultiplier;
            if (SnapMode == SnapMode.SnapValue)
            {
                diffVector = diffVector.RoundToDivisionValue(SnapValue);
            }

            _instructionUtility.UpdateInstruction(Mode, TransformationType);
            ScaleRaw(_selectionManager.SelectedNodes, diffVector, Mode, _cursor3D.Position);
        }

        public static void TranslateRaw(IEnumerable<INode> nodes, Vector3 diffVector, TransformationMode mode, Vector3 cursorPosition)
        {
            foreach (var node in nodes)
            {
                node.Transform.Translate(diffVector);
            }
        }
        public static void RotateRaw(IEnumerable<INode> nodes, Vector3 diffVector, TransformationMode mode, Vector3 cursorPosition)
        {
            foreach (var node in nodes)
            {
                if (mode == TransformationMode.Cursor)
                {
                    node.Transform.RotateAround(cursorPosition, diffVector);
                }
                else
                {
                    node.Transform.RotateAround(node.Transform.Position, diffVector);
                }
            }
        }
        public static void ScaleRaw(IEnumerable<INode> nodes, Vector3 diffVector, TransformationMode mode, Vector3 cursorPosition)
        {
            foreach (var node in nodes)
            {
                if (mode == TransformationMode.Cursor)
                {
                    node.Transform.ScaleAround(cursorPosition, diffVector);
                }
                else
                {
                    node.Transform.Scale += diffVector;
                }
            }
        }


        #endregion
    }
}
