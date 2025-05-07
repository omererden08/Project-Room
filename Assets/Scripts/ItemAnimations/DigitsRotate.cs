using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigitsRotate : MonoBehaviour
{
    [SerializeField] private float rotationDuration = 1f;
    [SerializeField] private List<Transform> digits = new List<Transform>();

    private Dictionary<Transform, bool> isRotating = new Dictionary<Transform, bool>();
    private Dictionary<Transform, float> currentXAngles = new Dictionary<Transform, float>();

    void Start()
    {
        if (digits.Count == 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                digits.Add(transform.GetChild(i));
            }
        }

        foreach (Transform digit in digits)
        {
            isRotating[digit] = false;
            currentXAngles[digit] = digit.localEulerAngles.x; // Baþlangýç açýsýný kaydet
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Sol týklama
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Transform hitTransform = hit.transform;

                if (digits.Contains(hitTransform) && !isRotating[hitTransform])
                {
                    StartCoroutine(RotateSingleDigit(hitTransform, rotationDuration));
                }
            }
        }

    }

    private IEnumerator RotateSingleDigit(Transform digit, float duration)
    {
        isRotating[digit] = true;

        float startX = currentXAngles[digit];
        float targetX = startX - 36f;
        currentXAngles[digit] = targetX; // Açýyý güncelle

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float currentX = Mathf.LerpAngle(startX, targetX, t);
            digit.localRotation = Quaternion.Euler(currentX, 270f, 270f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        digit.localRotation = Quaternion.Euler(targetX, 270f, 270f);
        CheckPassword();
        isRotating[digit] = false;
    }

    void CheckPassword()
    {
        float[] correctAngles = { 270f, 270f, 18f, 306f };
        float tolerance = 1f; // ±1 derece tolerans

        for (int i = 0; i < digits.Count; i++)
        {
            float angle = digits[i].localEulerAngles.x;

            // Açýnýn doðru aralýkta olup olmadýðýný kontrol et
            if (Mathf.Abs(Mathf.DeltaAngle(angle, correctAngles[i])) > tolerance)
            {
                Debug.Log("Password is incorrect.");
                return;
            }
        }

        Debug.Log("Password is correct!");
        
        // Þifre doðruysa yapýlacak iþlemler
        EvntManager.TriggerEvent("SafeOpen");

    }

}
