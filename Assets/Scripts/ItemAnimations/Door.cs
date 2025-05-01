using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
    private bool isMoving = false;
    [SerializeField] private LayerMask interactLayer;
    void Start()
    {
        
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
                    print("Hit door");
                    StartCoroutine(RotateDoor());
                }

            }
        }
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
