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
        if (!isMoving && isClicked)
        {
            StartCoroutine(MoveLever(!isOpen));

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
        float duration = 1f;

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


}
