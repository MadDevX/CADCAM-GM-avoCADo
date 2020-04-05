using OpenTK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public interface ITransform : INotifyPropertyChanged
    {
        Vector3 Position { get; set; }
        Quaternion Rotation { get; set; }
        Vector3 RotationEulerAngles { get; set; }
        Vector3 Scale { get; set; }

        INode Parent { get; set; }

        Matrix4 LocalModelMatrix { get; }
        Vector3 WorldPosition { get; set; }
        Vector2 ScreenCoords(Camera camera);

        void RotateAround(Vector3 pivot, Vector3 eulerAngles);
        void Translate(Vector3 translation);
        void ScaleAround(Vector3 pivot, Vector3 scaling);
    }
}
