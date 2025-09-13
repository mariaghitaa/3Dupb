using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;


public class CurveEditorUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] CurveCreator creator;          // tragem GO-ul cu CurveCreator
    [SerializeField] TMP_Dropdown curveTypeDropdown;
    [SerializeField] Toggle closedToggle;

    void Start()
    {
        // 1) asigură-te că lista este EXACT cele 3 opţiuni dorite
        curveTypeDropdown.ClearOptions();   // <- adăugat
        curveTypeDropdown.AddOptions(new List<string> { "Polyline", "CatmullRom", "BezierCubic" });

        // 2) sincronizează cu asset-ul
        curveTypeDropdown.value = (int)creator.CurveAsset.curveType;
        closedToggle.isOn = creator.CurveAsset.closed;

        // 3) ascultă schimbările
        curveTypeDropdown.onValueChanged.AddListener(OnCurveTypeChanged);
        closedToggle.onValueChanged.AddListener(OnClosedChanged);
    }


    void OnCurveTypeChanged(int idx)
    {
        CurveType newType = idx switch
        {
            0 => CurveType.Polyline,
            1 => CurveType.CatmullRom,
            2 => CurveType.BezierCubic,   // mapăm explicit la enum = 3
            _ => creator.CurveAsset.curveType
        };

        creator.CurveAsset.curveType = newType;
        creator.RefreshVisuals();
    }


    void OnClosedChanged(bool isClosed)
    {
        creator.CurveAsset.closed = isClosed;
        creator.RefreshVisuals();
    }
}
