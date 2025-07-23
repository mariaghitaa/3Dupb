using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
///  Editor de traiectorii în scenă – vers. minimală: clic-stânga adaugă puncte,
///  drag cu DraggableDot, Refresh() redesenează curba prin evaluare.
/// </summary>
public class CurveCreator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] CurveAsset curveAsset;          // asset salvat
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] GameObject dotPrefab;
    [SerializeField] Transform dotParent;
    public CurveAsset CurveAsset => curveAsset;

    [Header("Display")]
    [Range(8, 200)] public int resolution = 60;

    readonly List<Transform> dots = new();

    void Awake()
    {
        if (curveAsset == null)          // dacă n-a fost setat în Inspector
        {
            curveAsset = ScriptableObject.CreateInstance<CurveAsset>();
            curveAsset.name = "RuntimeCurve";
        }
        RefreshVisuals();
    }

    /*---------------- Input primitiv (demo) ----------------*/
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // ieșim când pointerul e peste UI
#if ENABLE_INPUT_SYSTEM
            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject(
                    UnityEngine.InputSystem.Pointer.current?.deviceId ?? -1))
                return;
#else
            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject())
                return;
#endif
            Vector3 w = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            w.z = 0f;
            AddPoint(w);
        }

        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Backspace))
            RemoveLastPoint();
    }

    /*---------------- API public ---------------------------*/
    public void AddPoint(Vector3 pos)
    {
        curveAsset.controlPoints.Add(pos);

        Transform d = Instantiate(dotPrefab, pos, Quaternion.identity, dotParent).transform;
        d.gameObject.name = $"Dot {curveAsset.controlPoints.Count}";
        dots.Add(d);

        RefreshVisuals();
    }
    public void RemoveLastPoint()
    {
        if (curveAsset.controlPoints.Count == 0) return;

        curveAsset.controlPoints.RemoveAt(curveAsset.controlPoints.Count - 1);
        Destroy(dots[^1].gameObject);
        dots.RemoveAt(dots.Count - 1);

        RefreshVisuals();
    }

    public void RefreshVisuals()
    {
        int segCount = curveAsset.controlPoints.Count - (curveAsset.closed ? 0 : 1);
        int totalSamples = resolution * Mathf.Max(1, segCount);   // e.g. 60 × (N-1)

        lineRenderer.positionCount = totalSamples;

        int k = 0;
        for (int s = 0; s < segCount; s++)
        {
            for (int j = 0; j < resolution; j++, k++)
            {
                float tSeg = j / (float)(resolution - 1);       // 0–1 pe segment
                float globalT = (s + tSeg) / segCount;          // 0–1 pe toată curba
                lineRenderer.SetPosition(k, curveAsset.Evaluate(globalT));
            }
        }

    }
}
