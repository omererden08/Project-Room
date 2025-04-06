using UnityEngine;

public class BallController : MonoBehaviour
{
    private Rigidbody rb;

    // Magnet variables
    [SerializeField] private GameObject[] magnet = new GameObject[3];
    [SerializeField] private int[] force = new int[3];
    [SerializeField] private int forceValue;
    [SerializeField] private LayerMask hitLayer;

    private Lever[] levers = new Lever[3];

    private readonly Vector3[] directions = { Vector3.right, Vector3.left, Vector3.up };

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        for (int i = 0; i < levers.Length; i++)
        {
            if (magnet[i] != null)
            {
                levers[i] = magnet[i].GetComponent<Lever>();
            }
        }
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

            int clickedIndex = -1;
            for (int i = 0; i < magnet.Length; i++)
            {
                if (magnet[i] == clickedObject)
                {
                    clickedIndex = i;
                    break;
                }
            }

            // Eğer daha önce eklenmemişse, boş slota ekle
            if (clickedIndex == -1)
            {
                for (int i = 0; i < magnet.Length; i++)
                {
                    if (magnet[i] == null)
                    {
                        magnet[i] = clickedObject;
                        clickedIndex = i;
                        break;
                    }
                }
            }

            // Artık tıklanan lever belli: clickedIndex
            if (clickedIndex != -1)
            {
                for (int i = 0; i < levers.Length; i++)
                {
                    if (levers[i] != null)
                    {
                        if (i == clickedIndex)
                        {
                            // Eğer zaten açıksa → kapat
                            if (levers[i].isOpen)
                            {
                                levers[i].isOpen = true;   // animasyonda !isOpen yapılacak
                                levers[i].isClicked = true;
                                force[i] = 0;
                            }
                            else
                            {
                                levers[i].isOpen = false;  // açılacak
                                levers[i].isClicked = true;
                                force[i] = forceValue;
                            }
                        }
                        else
                        {
                            // Diğer lever'lar kapanmalı
                            levers[i].isOpen = true; // kapanacak
                            levers[i].isClicked = true;
                            force[i] = 0;
                        }
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

        // Eğer yön yukarıysa (Vector3.up), force pozitif olur, diğer yönlerde negatif olur
        int forceValue = (directions[directionIndex] == Vector3.up) ? force[directionIndex] : -force[directionIndex];

        rb.AddForce(directions[directionIndex] * forceValue);
    }



}
