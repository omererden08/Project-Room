using UnityEngine;
using System.Collections;

public class Lever : MonoBehaviour
{
    private Quaternion targetRotation;
    private Vector3 targetPosition;
    private Quaternion initialRotation;
    private Vector3 initialPosition;
    public bool isOpen = false;

    [SerializeField] private float moveDuration;
    [SerializeField] private Transform leverSwitch;
    [Tooltip("Dogru olmasina diggat et")]
    public PuzzleManager puzzleManager;
    public bool isClicked = false;
    private bool isMoving = false;

    private void Start()
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag("LeverSwitch"))
            {
                leverSwitch = child;
                break;
            }
        }

        initialRotation = leverSwitch.localRotation;
        initialPosition = leverSwitch.localPosition;

        targetRotation = Quaternion.Euler(0, 0, 35);
        targetPosition = initialPosition + Vector3.up * 3;
    }

    private void Update()
    {
        if (!isMoving && isClicked && puzzleManager.inPuzzleMode)
        {
            StartCoroutine(MoveLever(isOpen)); // isOpen'ı tersine çevirmeden doğrudan kullan
            isClicked = false; // Reset isClicked to prevent multiple clicks
        }
    }

    private IEnumerator MoveLever(bool open)
    {
        isMoving = true;
        Vector3 fromPos = leverSwitch.localPosition;
        Quaternion fromRot = leverSwitch.localRotation;

        Vector3 toPos = open ? targetPosition : initialPosition;
        Quaternion toRot = open ? targetRotation : initialRotation;

        float elapsedTime = 0f;
        float duration = moveDuration > 0 ? moveDuration : 1f; // moveDuration sıfır veya negatifse varsayılan 1 saniye

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            leverSwitch.localPosition = Vector3.Lerp(fromPos, toPos, t);
            leverSwitch.localRotation = Quaternion.Lerp(fromRot, toRot, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        leverSwitch.localPosition = toPos;
        leverSwitch.localRotation = toRot;

        isMoving = false;
        isOpen = open;
    }

    // Dışarıdan isMoving durumunu kontrol etmek için metod
    public bool IsMoving()
    {
        return isMoving;
    }
}