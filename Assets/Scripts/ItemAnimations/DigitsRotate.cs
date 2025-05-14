using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DigitsRotate : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float rotationDuration = 1f;
    [SerializeField] private List<Transform> digits = new List<Transform>();

    [Header("Sounds")]
    public AudioClip Locked;



    private Dictionary<Transform, bool> isRotating = new Dictionary<Transform, bool>();
    private Dictionary<Transform, float> currentXAngles = new Dictionary<Transform, float>();

    public Bomb bomb;

    void Start()
    {
        bomb = FindAnyObjectByType<Bomb>();
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
            currentXAngles[digit] = digit.localEulerAngles.x; // Ba�lang�� a��s�n� kaydet
            StartCoroutine(RotateSingleDigit(digit, rotationDuration));

        }

    }

    void Update()
    {

        if (Input.GetMouseButtonDown(0)) // Sol t�klama
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Transform hitTransform = hit.transform;

                if (digits.Contains(hitTransform) && !isRotating[hitTransform])
                {
                    if (!bomb.isBoombReady)
                    {
                        EvntManager.TriggerEvent("subID", "STRT_INTER_" + Random.Range(1, 5));
                        return;
                    }
                    StartCoroutine(RotateSingleDigit(hitTransform, rotationDuration));
                    AudioManager.Instance.audioSource.PlayOneShot(Locked);
                }
            }
        }

    }

    private IEnumerator RotateSingleDigit(Transform digit, float duration)
    {
        isRotating[digit] = true;

        float startX = currentXAngles[digit];
        float targetX = startX - 36f;
        currentXAngles[digit] = targetX; // A��y� g�ncelle

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
        float[] correctAngles = { 306f, 18f, 342f, 270f };
        float tolerance = 1f; // �1 derece tolerans

        for (int i = 0; i < digits.Count; i++)
        {
            float angle = digits[i].localEulerAngles.x;

            // A��n�n do�ru aral�kta olup olmad���n� kontrol et
            if (Mathf.Abs(Mathf.DeltaAngle(angle, correctAngles[i])) > tolerance)
            {
                Debug.Log("Password is incorrect.");
                return;
            }
        }

        Debug.Log("Password is correct!");

        // �ifre do�ruysa yap�lacak i�lemler
        EvntManager.TriggerEvent("SafeOpen");

    }

}
