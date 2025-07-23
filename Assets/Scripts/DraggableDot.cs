using UnityEngine;

public class DraggableDot : MonoBehaviour
{
    Camera mainCamera;
    CurveCreator creator;
    //bool isDragging;
    void Awake()
    {
        creator = GetComponentInParent<CurveCreator>();
        mainCamera = Camera.main;
    }

    // private void OnMouseDown()
    // {
    //     isDragging = true;
    // }

    // private void OnMouseUp()
    // {
    //     isDragging = false;
    // }
    void OnMouseDrag()
    {
        Vector3 w = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        w.z = 0f;
        transform.position = w;
        if (creator) creator.RefreshVisuals();
    }

    // private void Update()
    // {
    //     if (!isDragging) return;

    //     Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
    //     mousePosition.z = 0f;
    //     transform.position = mousePosition;

    //     // Update the line position if necessary
    //     if (line != null)
    //     {
    //         line.Refresh();
    //     }

    // }
}
