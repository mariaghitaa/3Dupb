using System;
using UnityEngine;

namespace TrajectoryIO
{
    [Serializable]
    public struct Vec3S
    {
        public float x, y, z;
        public Vec3S(Vector3 v) { x = v.x; y = v.y; z = v.z; }
        public Vector3 ToV3() => new(x, y, z);
    }

    [Serializable]
    public class CurveDTO
    {
        public string curveType;   // "Polyline", "CatmullRom", "Bezier"
        public bool closed;
        public Vec3S[] points;
    }
}
