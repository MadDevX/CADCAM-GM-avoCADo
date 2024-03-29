﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace avoCADo
{
    public class BezierGeomGroupNode : GroupNode<BezierGeneratorGeometry>
    {
        public override GroupNodeType GroupNodeType => GroupNodeType.Attachable;
        public BezierGeomGroupNode(WpfObservableRangeCollection<INode> childrenSource, IRenderer renderer, BezierGeneratorGeometry dependent, string name) : base(childrenSource, renderer, dependent, name)
        {
        }

        public BezierGeomGroupNode(ITransform transform, WpfObservableRangeCollection<INode> childrenSource, IRenderer renderer, BezierGeneratorGeometry dependent, string name) : base(transform, childrenSource, renderer, dependent, name)
        {
        }
    }

    public class IntersectionCurveData
    {
        public IntersectionCurveGroupNode Node { get; }
        public IntersectionCurve Curve { get; }

        public IntersectionCurveData(IntersectionCurveGroupNode node, IntersectionCurve curve)
        {
            Node = node;
            Curve = curve;
        }
    }
}
