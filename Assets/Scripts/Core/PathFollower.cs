using UnityEngine;

public enum OrientationMode { LookForward, Fixed, LookAtTarget }

[RequireComponent(typeof(Transform))]
public class PathFollower : MonoBehaviour
{
    [Header("References")]
    [SerializeField] CurveAsset path;


    [Header("Settings")]
    [Min(0.1f)] public float duration = 3f;
    public bool loop = true;

    [Header("Orientation")]
    public OrientationMode orientation = OrientationMode.LookForward;
    public Transform lookAtTarget;

    float time;

    [SerializeField] bool usePathChain = false;
    [SerializeField] PathChain pathChain;
    public bool reversePlayback = false;


    [SerializeField] public MotionProfile motion;
    public MotionProfile Motion
    {
        get => motion;
        set => motion = value;
    }

    void Update()
    {
        if (path == null || motion == null || path.controlPoints.Count < 2) return;

        time += Time.deltaTime;
        float tNorm = time / duration;

        if (tNorm > 1f)
        {
            if (!loop) { enabled = false; return; }
            time = 0f;
            tNorm = 0f;
        }

        float s = motion.Evaluate(tNorm);
        if (reversePlayback) s = 1f - s;
        Vector3 pos, tan;

        if (usePathChain && pathChain != null)
        {
            pos = pathChain.Evaluate(s);
            tan = pathChain.Tangent(s);
        }
        else
        {
            if (path == null) return;
            pos = path.Evaluate(s);
            tan = path.Tangent(s);
        }
        transform.position = pos;
        // orientation
        switch (orientation)
        {
            case OrientationMode.LookForward:
                if (tan.sqrMagnitude > 0.0001f) transform.right = tan.normalized;
                break;
            case OrientationMode.LookAtTarget:
                if (lookAtTarget != null)
                {
                    var dir = lookAtTarget.position - transform.position;
                    if (dir.sqrMagnitude > 0.0001f) transform.right = dir.normalized;
                }
                break;
            case OrientationMode.Fixed:
            default: break;
        }
    }
}
