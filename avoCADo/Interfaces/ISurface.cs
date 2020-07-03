﻿using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace avoCADo
{
    public interface ISurface
    {
        event Action ParametersChanged;
        Vector2 ParameterURange { get; }
        Vector2 ParameterVRange { get; }
        bool ULoop { get; } //TODO: update for cylindrical patches
        bool VLoop { get; } //TODO: update for cylindrical patches

        Vector3 GetVertex(float u, float v);

        Vector3 DerivU(float u, float v);
        Vector3 DerivUU(float u, float v);
        Vector3 DerivV(float u, float v);
        Vector3 DerivVV(float u, float v);
        Vector3 Twist(float u, float v);
        Vector3 Normal(float u, float v);

        IList<IntersectionCurve> BoundingCurves { get; }
    }
}
