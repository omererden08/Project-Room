using UnityEngine;
using System.Collections;

public class Credits : MonoBehaviour
{
    [SerializeField] private float rotationDuration = 0.5f;
    [SerializeField] private GameObject creditsObject; // Bu collider içeren obje
    [SerializeField] private GameObject creditsGears;
    private Animator gears;
    private bool isRotating = false;
    private bool isOpen = false;
    private bool isHovered = false;

    private void Start()
    {
        if (creditsObject == null)
        {
            Debug.LogError("Credits object is not assigned in the inspector.");
            return;
        }
        gears = creditsGears.GetComponent<Animator>();
        // Bu scriptin baðlý olduðu objeyi baþlangýç rotasyonuna getiriyoruz
        transform.localRotation = Quaternion.Euler(90, 0, 90);
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Hover kontrolü
            if (hit.transform == creditsObject.transform)
            {
                isHovered = true; // Mouse bu objenin üzerinde
                if (isHovered)
                {
                    gears.SetTrigger("panel");  //trigger bitmiyor ciksa bile 

                    Debug.Log("Mouse, Credits Object'in üzerinde.");

                    // Týklama kontrolü
                    if (Input.GetMouseButtonDown(0) && !isRotating)
                    {
                        StartCoroutine(RotateCredits(!isOpen));
                    }
                }
                
            }
            else
            {
                isHovered = false; // Mouse baþka bir objeye geçti
            }
        }
    }


    private IEnumerator RotateCredits(bool open)
    {
        isRotating = true;

        Quaternion fromRot = transform.localRotation;
        Quaternion toRot = Quaternion.Euler(90f, 0f, open ? 200f : 90f);

        float elapsedTime = 0f;
        float duration = Mathf.Max(rotationDuration, 0.01f);

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.localRotation = Quaternion.Slerp(fromRot, toRot, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = toRot;
        isRotating = false;
        isOpen = open;
    }
}
