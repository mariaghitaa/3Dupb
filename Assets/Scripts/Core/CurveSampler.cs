using System.Collections.Generic;
using UnityEngine;

public static class CurveSampler
{
    /* Polyline */
    public static Vector3 Polyline(List<Vector3> pts, bool closed, float t)
    {
        if (pts.Count == 0) return Vector3.zero;
        if (pts.Count == 1) return pts[0];

        int segCount = closed ? pts.Count : pts.Count - 1;

        t = Mathf.Clamp01(t);
        float scaled = t * segCount;
        int idx = Mathf.FloorToInt(scaled);
        float localT = scaled - idx;

        // when is closed
        int i0 = idx % pts.Count;
        int i1 = (idx + 1) % pts.Count;

        return Vector3.Lerp(pts[i0], pts[i1], localT);
    }

    public static Vector3 PolylineTangent(List<Vector3> pts, bool closed, float t)
    {
        const float eps = 0.001f;
        return (Polyline(pts, closed, t + eps) - Polyline(pts, closed, t)).normalized;
    }


    /* Catmull-Rom  */
    public static Vector3 CatmullRom(List<Vector3> pts, bool closed, float t)
    {
        int count = pts.Count;
        if (count < 2) return count == 1 ? pts[0] : Vector3.zero;

        // verify if the loop is closed or not and create local copies for ends if not
        List<Vector3> p = closed ? pts : new List<Vector3>(pts) { pts[^1] };
        if (!closed) p.Insert(0, pts[0]);
        int segCount = p.Count - (closed ? 0 : 3);

        float scaled = Mathf.Clamp01(t) * segCount;
        int i = Mathf.FloorToInt(scaled);
        float u = scaled - i;

        int p0 = Mod(i - 1, p.Count);
        int p1 = Mod(i, p.Count);
        int p2 = Mod(i + 1, p.Count);
        int p3 = Mod(i + 2, p.Count);

        return 0.5f * (2f * p[p1]
                     + (-p[p0] + p[p2]) * u
                     + (2f * p[p0] - 5f * p[p1] + 4f * p[p2] - p[p3]) * u * u
                     + (-p[p0] + 3f * p[p1] - 3f * p[p2] + p[p3]) * u * u * u);
    }

    public static Vector3 CatmullRomTangent(List<Vector3> pts, bool closed, float t)
    {
        const float eps = 0.001f;
        return (CatmullRom(pts, closed, t + eps) - CatmullRom(pts, closed, t)).normalized;
    }

    /* Bezier */
    public static Vector3 Bezier(List<Vector3> pts, float t)
    {
        if (pts.Count == 0) return Vector3.zero;
        if (pts.Count == 1) return pts[0];

        List<Vector3> work = new List<Vector3>(pts);
        t = Mathf.Clamp01(t);

        while (work.Count > 1)
        {
            for (int i = 0; i < work.Count - 1; i++)
                work[i] = Vector3.Lerp(work[i], work[i + 1], t);
            work.RemoveAt(work.Count - 1);
        }
        return work[0];
    }

    public static Vector3 BezierTangent(List<Vector3> pts, float t)
    {
        const float eps = 0.001f;
        return (Bezier(pts, t + eps) - Bezier(pts, t)).normalized;
    }

    /* Bezier cubic segmentat */

    public static Vector3 BezierCubic(List<Vector3> pts, int segIndex, float u)
    {
        // 4 points per segment
        int baseIdx = segIndex * 3;
        Vector3 p0 = pts[baseIdx];
        Vector3 p1 = pts[baseIdx + 1];
        Vector3 p2 = pts[baseIdx + 2];
        Vector3 p3 = pts[baseIdx + 3];

        float t = u;
        float inv = 1 - t;
        return inv * inv * inv * p0 +
               3 * inv * inv * t * p1 +
               3 * inv * t * t * p2 +
               t * t * t * p3;
    }

    public static Vector3 BezierCubicTangent(List<Vector3> pts, int segIndex, float u)
    {
        const float eps = 0.0005f;
        Vector3 a = BezierCubic(pts, segIndex, Mathf.Clamp01(u));
        Vector3 b = BezierCubic(pts, segIndex, Mathf.Clamp01(u + eps));
        return (b - a).normalized;
    }
    static int Mod(int a, int b) => (a % b + b) % b;
}

