using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CurveCreator : MonoBehaviour
{
    [Header("Data / Rendering")]
    [SerializeField] CurveAsset curveAsset;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] GameObject dotPrefab;
    [SerializeField] Transform dotParent;
    [Range(8, 200)] public int resolution = 60;
    public CurveAsset CurveAsset => curveAsset;

    [Header("UI Raycast")]
    [SerializeField] GraphicRaycaster uiRaycaster;
    [SerializeField] EventSystem eventSystem;

    readonly List<Transform> dots = new();

    void Awake()
    {
        if (curveAsset == null)
        {
            curveAsset = ScriptableObject.CreateInstance<CurveAsset>();
            curveAsset.name = "RuntimeCurve";
        }

        if (eventSystem == null)
            eventSystem = EventSystem.current
#if UNITY_2023_1_OR_NEWER
                ?? Object.FindFirstObjectByType<EventSystem>();
#else
                ?? Object.FindObjectOfType<EventSystem>();
#endif

        if (uiRaycaster == null)
        {
            var canvas = GetComponentInParent<Canvas>();
            if (canvas != null) uiRaycaster = canvas.GetComponent<GraphicRaycaster>();
            if (uiRaycaster == null)
            {
#if UNITY_2023_1_OR_NEWER
                uiRaycaster = Object.FindFirstObjectByType<GraphicRaycaster>();
#else
                uiRaycaster = Object.FindObjectOfType<GraphicRaycaster>();
#endif
            }
        }

        RefreshVisuals();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (IsOverUI()) return;
            var w = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            w.z = 0f;
            AddPoint(w);
        }

        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Backspace))
        {
            if (IsOverUI()) return;
            RemoveLastPoint();
        }
    }

    bool IsOverUI()
    {
        if (uiRaycaster != null && eventSystem != null)
        {
            var ped = new PointerEventData(eventSystem) { position = Input.mousePosition };
            var results = new List<RaycastResult>();
            uiRaycaster.Raycast(ped, results);
            if (results.Count > 0) return true;
        }
        if (EventSystem.current != null)
        {
            if (EventSystem.current.IsPointerOverGameObject()) return true;
            if (EventSystem.current.currentSelectedGameObject != null) return true;
        }
        return false;
    }

    public void AddPoint(Vector3 pos)
    {

        var pts = curveAsset.controlPoints;

        if (curveAsset.curveType != CurveType.BezierCubic)
        {
            pts.Add(pos);
            RefreshVisuals();
            return;
        }

        // 1,4,7,10... (3n + 1)
        if (pts.Count == 0)
        {
            pts.Add(pos);
        }
        else
        {
            int lastAnchorIndex = pts.Count - 1;

            if ((pts.Count - 1) % 3 != 0)
            {

                pts.Add(pos);
            }
            else
            {
                Vector3 A = pts[lastAnchorIndex];
                Vector3 B = pos;
                Vector3 dir = B - A;
                pts.Add(A + dir / 3f);
                pts.Add(A + 2f * dir / 3f);
                pts.Add(B);
            }
        }

        RefreshVisuals();
    }

    public void RemoveLastPoint()
    {
        var pts = curveAsset.controlPoints;
        if (pts.Count == 0) return;

        int removeCount = 1;
        if (curveAsset.curveType == CurveType.BezierCubic && pts.Count > 1)
            removeCount = Mathf.Min(3, pts.Count);

        pts.RemoveRange(pts.Count - removeCount, removeCount);
        RefreshVisuals();
    }

    void ClearDots()
    {
        for (int i = 0; i < dots.Count; i++)
            if (dots[i] != null) Destroy(dots[i].gameObject);
        dots.Clear();
    }

    void RebuildDotsFromAsset()
    {
        ClearDots();
        for (int i = 0; i < curveAsset.controlPoints.Count; i++)
        {
            var p = curveAsset.controlPoints[i];
            var t = Instantiate(dotPrefab, p, Quaternion.identity, dotParent).transform;
            t.gameObject.name = $"Dot {i + 1}";
            dots.Add(t);
        }
    }

    public void RefreshVisuals()
    {
        RebuildDotsFromAsset();

        var pts = curveAsset.controlPoints;
        if (pts == null || pts.Count == 0)
        {
            lineRenderer.positionCount = 0;
            return;
        }

        int segCount;
        switch (curveAsset.curveType)
        {
            case CurveType.BezierCubic:
                segCount = Mathf.Max(1, (pts.Count - 1) / 3);
                break;
            case CurveType.Polyline:
            case CurveType.CatmullRom:
            default:
                segCount = pts.Count - (curveAsset.closed ? 0 : 1);
                segCount = Mathf.Max(1, segCount);
                break;
        }

        int totalSamples = resolution * segCount;
        lineRenderer.positionCount = totalSamples;

        int k = 0;
        for (int s = 0; s < segCount; s++)
        {
            for (int j = 0; j < resolution; j++, k++)
            {
                float tSeg = j / (float)(resolution - 1);
                float globalT = (segCount == 1) ? tSeg : (s + tSeg) / segCount;
                lineRenderer.SetPosition(k, curveAsset.Evaluate(globalT));
            }
        }
    }
}
