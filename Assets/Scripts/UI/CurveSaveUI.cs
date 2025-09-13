using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class CurveSaveUI : MonoBehaviour
{
    [SerializeField] CurveCreator creator;
    [SerializeField] Button saveBtn;
    [SerializeField] Button loadBtn;

    string PathFile => Path.Combine(
      Application.persistentDataPath, "lastCurve.json");

    void Awake()
    {
        saveBtn.onClick.AddListener(Save);
        loadBtn.onClick.AddListener(Load);
    }

    void Save()
    {
        File.WriteAllText(PathFile, creator.CurveAsset.ToJson(true));
        Debug.Log($"Curve saved to {PathFile}");
    }

    void Load()
    {
        if (File.Exists(PathFile))
        {
            creator.CurveAsset.FromJson(File.ReadAllText(PathFile));
            creator.RefreshVisuals();
            Debug.Log("Curve loaded!");
        }
    }
}
