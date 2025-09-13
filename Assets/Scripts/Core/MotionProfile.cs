using UnityEngine;
[CreateAssetMenu(menuName = "Animation2D/Motion Profile")]
public class MotionProfile : ScriptableObject
{
    public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
    public float Evaluate(float t) => curve.Evaluate(Mathf.Clamp01(t));
}
