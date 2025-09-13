using UnityEngine;

public class image : MonoBehaviour
{
    public Camera cam;
    public float padding = 0.02f; // 2% extra, să nu rămână margini

    SpriteRenderer sr;

    void Reset() { cam = Camera.main; sr = GetComponent<SpriteRenderer>(); }
    void Awake() { if (!sr) sr = GetComponent<SpriteRenderer>(); if (!cam) cam = Camera.main; }

    void LateUpdate()
    {
        if (!cam || !sr || !sr.sprite) return;

        float h = cam.orthographicSize * 2f;
        float w = h * cam.aspect;

        Vector2 s = sr.sprite.bounds.size;
        float sx = (w / s.x) + padding;
        float sy = (h / s.y) + padding;

        transform.localScale = new Vector3(sx, sy, 1f);
        transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, transform.position.z);
    }
}