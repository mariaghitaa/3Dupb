using UnityEngine;

public class ParentLeg : MonoBehaviour
{
    public Vector3 localPos = Vector3.zero;
    public float zOffsetDeg = -90f;    // robot orientat spre dreapta
    public Vector3 localScale = new(0.3f, 0.3f, 1f);

    void LateUpdate()
    {
        if (!transform.parent) return;
        transform.localPosition = localPos;
        transform.localRotation = Quaternion.Euler(0, 0, zOffsetDeg);
        transform.localScale = localScale;
    }
}
