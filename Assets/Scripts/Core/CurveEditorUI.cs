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
        curveTypeDropdown.AddOptions(
            new List<string> { "Polyline", "CatmullRom", "Bezier" });

        // 2) sincronizează cu asset-ul
        curveTypeDropdown.value = (int)creator.CurveAsset.curveType;
        closedToggle.isOn = creator.CurveAsset.closed;

        // 3) ascultă schimbările
        curveTypeDropdown.onValueChanged.AddListener(OnCurveTypeChanged);
        closedToggle.onValueChanged.AddListener(OnClosedChanged);
    }


    void OnCurveTypeChanged(int idx)
    {
        creator.CurveAsset.curveType = (CurveType)idx;
        creator.RefreshVisuals();
    }

    void OnClosedChanged(bool isClosed)
    {
        creator.CurveAsset.closed = isClosed;
        creator.RefreshVisuals();
    }
}
