using UnityEngine;
using UnityEngine.UI; // Eðer UI ile mesaj göstereceksen

public class TubeController : MonoBehaviour
{
    private Camera cam;
    private GameObject selectedObject;
    private Vector3 offset;
    private float zCoord;

    [SerializeField] private Transform[] socketPositions;
    [SerializeField] private GameObject[] tubes;
    [SerializeField] private GameObject[] indicatorHands;
    private GameObject[] socketToTube;
    [SerializeField] private int[] indicatorValue = new int[4];
    [SerializeField] private int[] targetIndicatorValues = new int[4];

    private void Awake()
    {
        for (int i = 0; i < indicatorValue.Length; i++)
        {
            indicatorValue[i] = 0;
        }
        
    }

    void Start()
    {
        cam = Camera.main;
        socketToTube = new GameObject[socketPositions.Length];
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TrySelectObject();
            IncreaseValue();
        }

        if (Input.GetMouseButton(0) && selectedObject != null)
            DragSelectedObject();

        if (Input.GetMouseButtonUp(0) && selectedObject != null)
        {
            ReleasedObject(selectedObject);
            selectedObject = null;
        }
    }

    void TrySelectObject()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("Draggable"))
        {
            selectedObject = hit.collider.gameObject;
            zCoord = cam.WorldToScreenPoint(selectedObject.transform.position).z;
            offset = selectedObject.transform.position - GetWorldPosition();
        }
    }

    void DragSelectedObject()
    {
        Vector3 targetPos = GetWorldPosition() + offset;
        selectedObject.transform.position = targetPos;
    }

    Vector3 GetWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = zCoord;
        return cam.ScreenToWorldPoint(mousePos);
    }

    void ReleasedObject(GameObject selectedObject)
    {
        for (int i = 0; i < socketPositions.Length; i++)
        {
            if (Vector3.Distance(selectedObject.transform.position, socketPositions[i].position) < 0.5f)
            {
                selectedObject.transform.position = socketPositions[i].position;


                socketToTube[i] = selectedObject;

                CheckForList();
                return;
            }
        }
    }

    void CheckForList()
    {
        for (int i = 0; i < tubes.Length; i++)
        {
            if (socketToTube[i] == null || socketToTube[i] != tubes[i])
                return;
        }
        print("Siralama dogru");
    }

    void IncreaseValue()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("Indicator"))
        {
            for (int i = 0; i < indicatorHands.Length; i++)
            {
                if (hit.collider.gameObject == indicatorHands[i])
                {
                    indicatorValue[i]++;
                    if (indicatorValue[i] > 3)
                        indicatorValue[i] = 0;

                    // Optional: Debug veya görsel feedback için log
                    Debug.Log($"Indicator {i} deðeri: {indicatorValue[i]}");
                    break; // eþleþmeyi bulduk, döngüden çýkabiliriz
                }
            }
            CheckIndicatorValues();
        }
    }
    void CheckIndicatorValues()
    {
        for (int i = 0; i < indicatorValue.Length; i++)
        {
            if (indicatorValue[i] != targetIndicatorValues[i])
                return; // herhangi biri yanlýþsa çýk
        }

        Debug.Log("Doðru deðerler!"); // Hepsi doðruysa
    }

}
