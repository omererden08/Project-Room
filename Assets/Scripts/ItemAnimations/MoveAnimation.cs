using System.Collections;
using UnityEngine;

public class MoveAnimation : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Transform targetPos;
    private float moveDuration = 0.55f;
    private float platformDuration = 4f;
    private float delayTime; // Delay for bomb after platform

    [Header("Interaction")]
    [SerializeField] private LayerMask interactLayer;

    private Vector3 initialPos;
    private bool isMoving = false;
    private bool isOpen = false;

    void Start()
    {
        initialPos = transform.position;

        if (targetPos == null && transform.childCount > 0)
        {
            targetPos = transform.GetChild(0);
        }

        if (interactLayer.value == 0)
        {
            Debug.LogWarning($"{name}: InteractLayer is not set in the inspector.");
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isMoving)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, interactLayer))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    ToggleDrawer(moveDuration);
                }
            }
        }

        if (Input.GetMouseButtonDown(0) && CompareTag("Door") && !isMoving)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, interactLayer))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    print("Hit door");  
                    StartCoroutine(RotateDoor());
                }

            }
        }

        if (Input.GetKeyDown(KeyCode.P) && CompareTag("Platform") && !isMoving)
        {
            ToggleDrawer(platformDuration);
        }
    }

    void ToggleDrawer(float duration)
    {
        isOpen = !isOpen;
        StartCoroutine(Move(isOpen, duration));
    }


    private IEnumerator Move(bool open, float duration)
    {
        isMoving = true;

        Vector3 start = transform.position;
        Vector3 end = open ? targetPos.position : initialPos;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.position = Vector3.Lerp(start, end, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = end;
        isMoving = false;
    }

    IEnumerator RotateDoor()
    {
        isMoving = true;

        Quaternion fromRotation = Quaternion.Euler(-90f, 0f, 90f);
        Quaternion toRotation = Quaternion.Euler(-100f, 0f, 90f);
        float duration = 0.1f;
        float elapsedTime = 0f;

        // Forward rotation (from -> to)
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.rotation = Quaternion.Slerp(fromRotation, toRotation, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Make sure it finishes at exact target
        transform.rotation = toRotation;

        // Optional: small pause at target
        yield return new WaitForSeconds(0.1f);

        // Reverse rotation (to -> from)
        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.rotation = Quaternion.Slerp(toRotation, fromRotation, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final rotation is exactly the starting one
        transform.rotation = fromRotation;

        isMoving = false;
    }
}
