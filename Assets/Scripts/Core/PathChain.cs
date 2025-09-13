using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Animation2D/Path Chain", fileName = "PathChain")]
public class PathChain : ScriptableObject
{
    [System.Serializable]
    public class Entry
    {
        public CurveAsset curve;
        public bool reverse;
    }

    public List<Entry> entries = new();
    [Range(8, 256)] public int samplingResolution = 64;

    List<float> cumulative = new();
    float totalLength;

    void OnValidate() { BuildLookup(); }

    float CurveLength(CurveAsset c)
    {
        if (c == null || c.controlPoints.Count < 2) return 0f;
        int n = Mathf.Max(2, samplingResolution);
        Vector3 prev = c.Evaluate(0f);
        float len = 0f;
        for (int i = 1; i < n; i++)
        {
            float t = i / (float)(n - 1);
            Vector3 p = c.Evaluate(t);
            len += Vector3.Distance(prev, p);
            prev = p;
        }
        return len;
    }

    public void BuildLookup()
    {
        cumulative.Clear(); totalLength = 0f;
        foreach (var e in entries)
        {
            float l = (e != null && e.curve != null) ? CurveLength(e.curve) : 0f;
            totalLength += l;
            cumulative.Add(totalLength);
        }
        if (totalLength <= 0f) cumulative.Clear();
    }

    void Ensure()
    {
        if (cumulative == null || cumulative.Count != entries.Count || totalLength <= 0f)
            BuildLookup();
    }

    int FindIndex(float tNorm, out float localT)
    {
        Ensure();
        if (entries.Count == 0 || totalLength <= 0f) { localT = 0f; return -1; }

        float target = Mathf.Clamp01(tNorm) * totalLength;
        int idx = 0;
        while (idx < cumulative.Count && cumulative[idx] < target) idx++;

        float prevCum = idx == 0 ? 0f : cumulative[idx - 1];
        float segLen = Mathf.Max(1e-6f, cumulative[idx] - prevCum);
        localT = Mathf.InverseLerp(prevCum, cumulative[idx], target);
        return idx;
    }

    public Vector3 Evaluate(float t)
    {
        int idx = FindIndex(t, out float u);
        if (idx < 0) return Vector3.zero;

        var e = entries[idx];
        float s = e.reverse ? 1f - u : u;
        return e.curve ? e.curve.Evaluate(s) : Vector3.zero;
    }

    public Vector3 Tangent(float t)
    {
        int idx = FindIndex(t, out float u);
        if (idx < 0) return Vector3.right;

        var e = entries[idx];
        float s = e.reverse ? 1f - u : u;
        var tan = e.curve ? e.curve.Tangent(s) : Vector3.right;
        return e.reverse ? -tan : tan;
    }
}
