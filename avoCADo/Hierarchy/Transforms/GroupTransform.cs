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

        public override INode Node
        {
            get => base.Node;
            set
            {
                //Unsubscribe from previous parent
                var prevParent = Node; 
                if(prevParent != null)
                {
                    prevParent.PropertyChanged -= Node_PropertyChanged;
                    prevParent.Children.CollectionChanged -= Children_CollectionChanged;
                }
                //Subscribe to new parent
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

        public override Vector2 ScreenCoords(Camera camera)
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

        public override void RotateAround(Vector3 pivot, Vector3 eulerAngles)
        {
            if (NodeSelection.Manager.SelectedNodes.Count == 1 && NodeSelection.Manager.MainSelection == Node)
            {
                foreach (var child in Node.Children)
                {
                    child.Transform.RotateAround(pivot, eulerAngles);
                }
            }
        }

        public override void ScaleAround(Vector3 pivot, Vector3 scaling)
        {
            if (NodeSelection.Manager.SelectedNodes.Count == 1 && NodeSelection.Manager.MainSelection == Node)
            {
                foreach (var child in Node.Children)
                {
                    child.Transform.ScaleAround(pivot, scaling);
                }
            }
        }

        public override void Translate(Vector3 translation)
        {
            if (NodeSelection.Manager.SelectedNodes.Count == 1 && NodeSelection.Manager.MainSelection == Node)
            {
                foreach (var child in Node.Children)
                {
                    child.Transform.Translate(translation);
                }
            }
        }

        public override void TranslateSnapped(Vector3 translation, float snapValue)
        {
            if (NodeSelection.Manager.SelectedNodes.Count == 1 && NodeSelection.Manager.MainSelection == Node)
            {
                foreach (var child in Node.Children)
                {
                    child.Transform.TranslateSnapped(translation, snapValue);
                }
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
        }
    }
}