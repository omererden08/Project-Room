using UnityEngine;

public class BallController : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private GameObject[] magnet = new GameObject[3];
    [SerializeField] private int[] force = new int[3];
    [SerializeField] private int forceValue;
    [SerializeField] private LayerMask hitLayer;
    private int selectedMagnetIndex = 0;


    private readonly Vector3[] directions = { Vector3.right, Vector3.left, Vector3.up };

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Magnet();
        }

        MoveAll();

    }

    public void Magnet()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, hitLayer))
        {
            GameObject clickedObject = hit.collider.gameObject;

            int existingIndex = -1;
            for (int i = 0; i < magnet.Length; i++)
            {
                if (magnet[i] == clickedObject)
                {
                    existingIndex = i;
                    break;
                }
            }

            if (existingIndex != -1)
            {
                for (int i = 0; i < force.Length; i++)
                {
                    force[i] = (i == existingIndex) ? forceValue : 0;
                }
            }
            else
            {
                for (int i = 0; i < magnet.Length; i++)
                {
                    if (magnet[i] == null)
                    {
                        magnet[i] = clickedObject;

                        for (int j = 0; j < force.Length; j++)
                        {
                            force[j] = (j == i) ? forceValue : 0;
                        }

                        break;
                    }
                }
            }
        }
    }



    void MoveAll()
    {
        for (int i = 0; i < magnet.Length; i++)
        {
            if (magnet[i] != null)
            {
                Move(i);
            }
        }
    }



    void Move(int directionIndex)
    {
        // Geçerli bir index mi kontrol edelim.
        if (directionIndex < 0 || directionIndex >= directions.Length || magnet[directionIndex] == null)
        {
            Debug.LogWarning("Invalid direction index or no magnet assigned.");
            return;
        }

        // Eðer yön yukarýysa (Vector3.up), force pozitif olur, diðer yönlerde negatif olur
        int forceValue = (directions[directionIndex] == Vector3.up) ? force[directionIndex] : -force[directionIndex];

        rb.AddForce(directions[directionIndex] * forceValue);
    }
}
