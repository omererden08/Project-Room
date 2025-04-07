using System.Collections;
using UnityEngine;

public class Drawer : MonoBehaviour
{
    [SerializeField] private Transform targetPos;
    [SerializeField] private float moveDuration = 1f;

    private Vector3 initialPos;
    private bool isMoving = false;
    private bool isOpen = false;
    [SerializeField] LayerMask interactLayer;

    void Start()
    {
        initialPos = transform.position;

        if (targetPos == null)
        {
            targetPos = transform.GetChild(0);
        }
        interactLayer = LayerMask.GetMask("Interactable");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isMoving)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, interactLayer))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    isOpen = !isOpen;
                    StartCoroutine(Move(isOpen));
                    print("Clicked on the bookshelf");
                }

            }
        }
        /*if (Input.GetKeyDown(KeyCode.E) && !isMoving)
        {
            isOpen = !isOpen;
            StartCoroutine(Move(isOpen));
            print("Clicked on the bookshelf");
        }*/
    }




    private IEnumerator Move(bool open)
    {
        isMoving = true;

        Vector3 fromPos = transform.position;
        Vector3 toPos = open ? targetPos.position : initialPos;

        float elapsedTime = 0f;
        float duration = moveDuration > 0 ? moveDuration : 1f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.position = Vector3.Lerp(fromPos, toPos, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = toPos;

        isMoving = false;
        isOpen = open;
    }
}
