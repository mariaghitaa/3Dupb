using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MotionUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] PathFollower follower;
    [SerializeField] TMP_Dropdown profileDropdown;
    [SerializeField] Slider durationSlider;
    [SerializeField] TMP_Text durationLabel;

    [Header("Profiles")]
    [SerializeField] MotionProfile constantProfile;
    [SerializeField] MotionProfile easeProfile;
    [SerializeField] MotionProfile stopGoProfile;
    [SerializeField] MotionProfile bounceProfile;
    [SerializeField] MotionProfile customProfile;

    void Start()
    {
        // populăm dropdownul
        profileDropdown.ClearOptions();
        profileDropdown.AddOptions(
            new List<string> { "Constant", "Ease-In-Out", "Stop-Go", "Bounce", "Custom" });

        profileDropdown.onValueChanged.AddListener(OnProfileChanged);
        durationSlider.onValueChanged.AddListener(OnDurationChanged);

        // iniţializare valori
        profileDropdown.value = 0;
        follower.motion = constantProfile;

        durationSlider.value = follower.duration;
        durationLabel.text = $"{follower.duration:0.0}s";
    }

    void OnProfileChanged(int idx)
    {
        follower.motion = idx switch
        {
            0 => constantProfile,
            1 => easeProfile,
            2 => stopGoProfile,
            3 => bounceProfile,
            _ => customProfile
        };
    }

    void OnDurationChanged(float v)
    {
        follower.duration = v;
        durationLabel.text = $"{v:0.0}s";
    }
}
