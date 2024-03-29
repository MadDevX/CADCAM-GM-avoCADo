﻿using OpenTK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    class GroupTransform : DummyTransform
    {
        private Vector3 _averageChildWorldPosition;
        private static PropertyChangedEventArgs _positionChangedArgs = new PropertyChangedEventArgs(nameof(Position));
        public override INode Node
        {
            get => base.Node;
            set
            {
                //Unsubscribe from previous owner
                var prevParent = Node; 
                if(prevParent != null)
                {
                    prevParent.PropertyChanged -= Node_PropertyChanged;
                    prevParent.Children.CollectionChanged -= Children_CollectionChanged;
                }
                //Subscribe to new owner
                base.Node = value;
                if (Node != null)
                {
                    Node.PropertyChanged += Node_PropertyChanged;
                    Node.Children.CollectionChanged += Children_CollectionChanged;
                }
            }
        }

        public override Vector3 Position { get => _averageChildWorldPosition; set => base.Position = value; }
        public override Vector3 WorldPosition { get => _averageChildWorldPosition; set => base.WorldPosition = value; }

        public override Vector2 ScreenCoords(ICamera camera)
        {
            if(Node.GroupNodeType != GroupNodeType.None)
            {
                return Coordinates.ScreenCoords(camera, _averageChildWorldPosition);
            }
            else
            {
                return base.ScreenCoords(camera);
            }
        }

        private void Children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateAveragePosition();
        }

        private void Node_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateAveragePosition();
        }

        private void UpdateAveragePosition()
        {
            _averageChildWorldPosition = Vector3.Zero;
            foreach(var child in Node.Children) //TODO: not numerically stable, check if it is an issue
            {
                _averageChildWorldPosition += child.Transform.WorldPosition;
            }
            _averageChildWorldPosition /= Node.Children.Count;
            RaisePropertyChanged(this, _positionChangedArgs);
        }
    }
}
