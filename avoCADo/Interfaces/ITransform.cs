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
        /// <summary>
        /// Returns previous parent as a parameter.
        /// </summary>
        event Action<INode> ParentChanged;

        Vector3 Position { get; set; }
        Quaternion Rotation { get; set; }
        Vector3 RotationEulerAngles { get; set; }
        Vector3 Scale { get; set; }


        /// <summary>
        /// Represents parent of the Node that owns this Transform. Should be moved to INode.
        /// </summary>
        INode ParentNode { get; set; } //TODO: move this to INode interface

        /// <summary>
        /// Represents Node that owns this Transform. Reassigning Node to a different one is not allowed.
        /// </summary>
        INode Node { get; set; }

        Matrix4 LocalModelMatrix { get; }
        Vector3 WorldPosition { get; set; }
        Vector2 ScreenCoords(ICamera camera);

        void RotateAround(Vector3 pivot, Vector3 eulerAngles);
        void Translate(Vector3 translation);
        Vector3 TranslateSnapped(Vector3 translation, float snapValue);
        void ScaleAround(Vector3 pivot, Vector3 scaling);
    }
}
