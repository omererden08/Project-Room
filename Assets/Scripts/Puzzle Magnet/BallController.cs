using UnityEngine;

public class BallController : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField]
    private GameObject[] magnet = new GameObject[3];
    [SerializeField]
    private int[] force = new int[3];
    public LayerMask hitLayer;

    private readonly Vector3[] directions = { Vector3.right, Vector3.left, Vector3.up };

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            MagnetIncrease();
        }
        if (Input.GetMouseButtonDown(1))
        {
            MagnetDecrease();
        }

        MoveAll();

    }

    public void MagnetIncrease()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, hitLayer))
        {

            for (int i = 0; i < magnet.Length; i++)
            {
                if (magnet[i] == hit.collider.gameObject)
                {
                    if (force[i] <= 9)
                    {
                        force[i] += 3;
                    }
                    else
                    {
                        force[i] = 0;
                    }

                    return;
                }
            }

            // Boþ bir magnet slotu bul ve atama yap
            for (int i = 0; i < magnet.Length; i++)
            {
                if (magnet[i] == null)
                {
                    magnet[i] = hit.collider.gameObject;
                    force[i] = 5;
                    return;
                }
            }
        }
    }
    public void MagnetDecrease()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, hitLayer))
        {

            for (int i = 0; i < magnet.Length; i++)
            {
                if (magnet[i] == hit.collider.gameObject)
                {
                    if (force[i] >= 0)
                    {
                        force[i] -= 3;
                    }
                    else
                    {
                        force[i] = 0;
                    }

                    return;
                }
            }

            // Boþ bir magnet slotu bul ve atama yap
            for (int i = 0; i < magnet.Length; i++)
            {
                if (magnet[i] == null)
                {
                    magnet[i] = hit.collider.gameObject;
                    force[i] = 5;
                    return;
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
