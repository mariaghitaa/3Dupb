#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CurveAsset))]
public class CurveAssetEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var asset = (CurveAsset)target;

        GUILayout.Space(8);
        using (new GUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Export JSON"))
            {
                string p = EditorUtility.SaveFilePanel(
                    "Export Curve", "Assets", asset.name, "json");
                if (!string.IsNullOrEmpty(p)) { asset.ExportJson(p); AssetDatabase.Refresh(); }
            }
            if (GUILayout.Button("Import JSON"))
            {
                string p = EditorUtility.OpenFilePanel(
                    "Import Curve", "Assets", "json");
                if (!string.IsNullOrEmpty(p)) { asset.ImportJson(p); }
            }
        }
    }
}
#endif
