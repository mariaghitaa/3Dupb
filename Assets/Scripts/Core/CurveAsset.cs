using System.Collections.Generic;
using UnityEngine;

public enum CurveType { Polyline, CatmullRom, Bezier }

/// <summary>Stochează punctele de control + felul curbei.</summary>
[CreateAssetMenu(menuName = "Animation2D/Curve Asset")]
public class CurveAsset : ScriptableObject
{
    public CurveType curveType = CurveType.Polyline;
    public bool closed = false;                 // pentru loop
    public List<Vector3> controlPoints = new();        // în coordonate world 2D

    // ====================== API de eșantionare ====================== //
    public Vector3 Evaluate(float t) => curveType switch
    {
        CurveType.Polyline => CurveSampler.Polyline(controlPoints, closed, t),
        CurveType.CatmullRom => CurveSampler.CatmullRom(controlPoints, closed, t),
        CurveType.Bezier => CurveSampler.Bezier(controlPoints, t),
        _ => Vector3.zero
    };

    public Vector3 Tangent(float t) => curveType switch
    {
        CurveType.Polyline => CurveSampler.PolylineTangent(controlPoints, closed, t),
        CurveType.CatmullRom => CurveSampler.CatmullRomTangent(controlPoints, closed, t),
        CurveType.Bezier => CurveSampler.BezierTangent(controlPoints, t),
        _ => Vector3.right
    };
}
