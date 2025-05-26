using UnityEngine;
using System.Collections;

public class Credits : MonoBehaviour
{
    [SerializeField] private float rotationDuration = 0.5f;
    [SerializeField] private GameObject creditsObject; // Bu collider i�eren obje
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

        gears.SetBool("start", true);
        // Bu scriptin ba�l� oldu�u objeyi ba�lang�� rotasyonuna getiriyoruz
        transform.localRotation = Quaternion.Euler(90, 0, 90);
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Hover kontrol�
            if (hit.transform == creditsObject.transform)
            {

                if (Input.GetMouseButtonDown(0) && !isRotating)
                {
                    StartCoroutine(RotateCredits(!isOpen));
                }
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
            gears.SetBool("start", false);
            float t = elapsedTime / duration;
            transform.localRotation = Quaternion.Slerp(fromRot, toRot, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        gears.SetBool("start", true);

        transform.localRotation = toRot;
        isRotating = false;
        isOpen = open;
    }
}
