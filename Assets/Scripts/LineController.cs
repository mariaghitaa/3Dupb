using UnityEngine;
using System.Collections.Generic;

public class LineController : MonoBehaviour
{
    // –––––– Inspector references ––––––
    [SerializeField] GameObject dotPrefab;   // necesar dacă adaugi punctele chiar din acest script
    [SerializeField] Transform dotParent;   // parent pentru puncte (poate fi chiar acest GO)

    // –––––– Runtime ––––––
    LineRenderer lr;
    public List<Transform> points = new();   // lăsăm public => DraggableDot vede lista

    /*==================== INITIALIZARE ====================*/
    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 0;
        lr.startWidth = lr.endWidth = 0.12f;
        lr.numCapVertices = 8;             // capete rotunjite
    }

    /*==================== INPUT PRINCIPAL =================*/
    void Update()
    {
        /*  CLIC STÂNGA – adaugă punct (doar dacă vrei să gestionezi adăugarea aici).
            Dacă adaugi punctele din alt script (ex. Pane Tool), ignoră acest bloc.
        */
        if (Input.GetMouseButtonDown(0) && dotPrefab != null)
        {
            Vector3 world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            world.z = 0f;

            Transform p = Instantiate(dotPrefab, world, Quaternion.identity, dotParent).transform;
            AddPoint(p);
        }

        /*  UNDO: clic-dreapta SAU Backspace  */
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Backspace))
            RemoveLastPoint();
    }

    /*==================== API PUBLIC ======================*/
    public void AddPoint(Transform point)
    {
        points.Add(point);

        lr.positionCount = points.Count;
        lr.SetPosition(points.Count - 1, point.position);   // actualizăm imediat
    }

    public void RemoveLastPoint()
    {
        if (points.Count == 0) return;

        // distrugem punctul din scenă
        Transform last = points[^1];
        points.RemoveAt(points.Count - 1);
        Destroy(last.gameObject);

        lr.positionCount = points.Count;
        Refresh();                                          // redesenăm
    }

    /// <summary>
    ///  Re-creează linia trecând prin toate punctele curente.
    ///  Este chemată de DraggableDot când muți un punct.
    /// </summary>
    public void Refresh()
    {
        for (int i = 0; i < points.Count; i++)
            lr.SetPosition(i, points[i].position);
    }

    /*==================== OPTIONAL ========================*/
    // Dacă vrei ca linia să “urmărească” punctele chiar și când le muți din Inspector,
    // poți lăsa și acest LateUpdate (nu e obligatoriu dacă te bazezi pe Refresh()).
    // void LateUpdate() => Refresh();
}
