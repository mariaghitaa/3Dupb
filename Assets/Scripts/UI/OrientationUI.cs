using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrientationUI : MonoBehaviour
{
    [SerializeField] PathFollower follower;
    [SerializeField] TMP_Dropdown orientDropdown;
    [SerializeField] Button pickTargetBtn;   // optional

    void Start()
    {
        orientDropdown.ClearOptions();
        orientDropdown.AddOptions(
            new System.Collections.Generic.List<string>
            { "Look Forward", "Fixed", "Look At Target" });

        orientDropdown.value = (int)follower.orientation;
        orientDropdown.onValueChanged.AddListener(OnOrientChanged);

        if (pickTargetBtn != null)
            pickTargetBtn.onClick.AddListener(PickTarget);
    }

    void OnOrientChanged(int idx)
    {
        follower.orientation = (OrientationMode)idx;
    }

    void PickTarget()
    {
#if UNITY_EDITOR
        follower.lookAtTarget = UnityEditor.Selection.activeTransform;
#endif
    }
}
