using UnityEngine;
using System.Collections;

public class Lever : MonoBehaviour
{
    private Quaternion targetRotation;
    private Quaternion initialRotation;
    [SerializeField] private Outline3D outline; // Outline bileşenini referans al

    [SerializeField] private float moveDuration;
    
    public bool isOpen = false;
    public bool isClicked = false;
    private bool isMoving = false;

    void Start()
    {
        outline = GetComponent<Outline3D>();
        outline.enabled = false; // Başlangıçta outline'ı gizle
        // Başlangıçta localRotation.z = -20 dereceye ayarla
        initialRotation = Quaternion.Euler(0f, 0f, -20f);
        targetRotation = Quaternion.Euler(0f, 0f, 20f);

        transform.localRotation = initialRotation;
    }


    private void Update()
    {
        if (!isMoving && isClicked)
        {
            StartCoroutine(MoveLever(isOpen)); // isOpen'ı tersine çevirmeden doğrudan kullan
            isClicked = false; // Reset isClicked to prevent multiple clicks
        }
    }

    private IEnumerator MoveLever(bool open)
    {
        isMoving = true;
        Quaternion fromRot = transform.localRotation;
        Quaternion toRot = open ? targetRotation : initialRotation;

        float elapsedTime = 0f;
        float duration = moveDuration > 0 ? moveDuration : 1f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.localRotation = Quaternion.Slerp(fromRot, toRot, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = toRot;
        isMoving = false;
        isOpen = open;

    }


    // Dışarıdan isMoving durumunu kontrol etmek için metod
    public bool IsMoving()
    {
        return isMoving;
    }
}