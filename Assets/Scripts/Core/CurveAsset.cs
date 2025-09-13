using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TrajectoryIO;
using System;

public enum CurveType { Polyline, CatmullRom, BezierGlobal, BezierCubic }
[CreateAssetMenu(menuName = "Animation2D/Curve Asset")]
public partial class CurveAsset : ScriptableObject
{
    public CurveType curveType = CurveType.Polyline;
    public bool closed = false;
    public List<Vector3> controlPoints = new();

    public Vector3 Evaluate(float t) => curveType switch
    {
        CurveType.Polyline => CurveSampler.Polyline(controlPoints, closed, t),
        CurveType.CatmullRom => CurveSampler.CatmullRom(controlPoints, closed, t),
        CurveType.BezierGlobal => CurveSampler.Bezier(controlPoints, t),
        CurveType.BezierCubic => EvaluateBezierCubic(t),

        _ => Vector3.zero
    };

    public Vector3 Tangent(float t) => curveType switch
    {
        CurveType.Polyline => CurveSampler.PolylineTangent(controlPoints, closed, t),
        CurveType.CatmullRom => CurveSampler.CatmullRomTangent(controlPoints, closed, t),
        CurveType.BezierGlobal => CurveSampler.BezierTangent(controlPoints, t),
        CurveType.BezierCubic => TangentBezierCubic(t),
        _ => Vector3.right
    };

    // helpers for BezierCubic
    Vector3 EvaluateBezierCubic(float t)
    {
        int segCount = (controlPoints.Count - 1) / 3;      // 4 points/segment
        if (segCount < 1) return controlPoints[0];

        float scaled = Mathf.Clamp01(t) * segCount;
        int segIdx = Mathf.FloorToInt(scaled);
        segIdx = Mathf.Min(segIdx, segCount - 1);
        float u = scaled - segIdx;

        return CurveSampler.BezierCubic(controlPoints, segIdx, u);
    }

    Vector3 TangentBezierCubic(float t)
    {
        int segCount = (controlPoints.Count - 1) / 3;
        if (segCount < 1) return Vector3.right;

        float scaled = Mathf.Clamp01(t) * segCount;
        int segIdx = Mathf.FloorToInt(scaled);
        segIdx = Mathf.Min(segIdx, segCount - 1);
        float u = scaled - segIdx;

        return CurveSampler.BezierCubicTangent(controlPoints, segIdx, u);
    }

}

public partial class CurveAsset
{
    const string Ext = ".json";

    public string ToJson(bool pretty = true)
    {
        var dto = new CurveDTO
        {
            curveType = curveType.ToString(),
            closed = closed,
            points = controlPoints.ConvertAll(p => new Vec3S(p)).ToArray()
        };
        return JsonUtility.ToJson(dto, pretty);
    }

    public void FromJson(string json)
    {
        var dto = JsonUtility.FromJson<CurveDTO>(json);
        curveType = Enum.Parse<CurveType>(dto.curveType);
        closed = dto.closed;
        controlPoints.Clear();
        foreach (var p in dto.points) controlPoints.Add(p.ToV3());
    }

#if UNITY_EDITOR
    public void ExportJson(string path) => File.WriteAllText(path, ToJson(true));
    public void ImportJson(string path)
    {
        if (File.Exists(path)) FromJson(File.ReadAllText(path));
    }
#endif
}