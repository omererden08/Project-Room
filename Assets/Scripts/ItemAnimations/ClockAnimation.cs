using System.Collections;
using UnityEngine;

public class ClockAnimation : IInteractable
{
    [Header("Token")]
    [SerializeField] private GameObject token;

    [Header("Clock Hands")]
    [SerializeField] private Transform clockBigHand;
    [SerializeField] private Transform clockSmallHand;

    [Header("Settings")]
    [SerializeField] private float rotateDuration = 1f;
    [SerializeField] private float valveYRotation = -25f;
    [SerializeField] private float bigHandYRotation = -90f;
    [SerializeField] private float smallHandYRotation = -90f;
    [SerializeField] private float bigHandRotationAmount = 180f;
    [SerializeField] private float smallHandRotationAmount = 15f;

    private LayerMask interactLayer;
    private bool isRotating = false;
    private TeaLever tL;

    private void Start()
    {
        outline = GetComponent<Outline3D>();
        outline.enabled = false;
        tL = FindAnyObjectByType<TeaLever>();
        interactLayer = 1 << gameObject.layer;
        token.SetActive(false);
    }

    public override void Interact()
    {
        if (!isRotating)
        {
            StartCoroutine(RotateValveAndClock());
        }

    }



    private IEnumerator RotateValveAndClock()
    {
        isRotating = true;

        float elapsedTime = 0f;

        float startValveZ = transform.eulerAngles.z;
        float endValveZ = startValveZ + 360f;

        float startBigZ = clockBigHand.eulerAngles.z;
        float endBigZ = startBigZ + bigHandRotationAmount;

        float startSmallZ = clockSmallHand.eulerAngles.z;
        float endSmallZ = startSmallZ + smallHandRotationAmount;

        while (elapsedTime < rotateDuration)
        {
            float t = elapsedTime / rotateDuration;

            // Valve Rotation
            float currentValveZ = Mathf.Lerp(startValveZ, endValveZ, t);
            transform.rotation = Quaternion.Euler(0f, valveYRotation + 45f, currentValveZ);

            // Clock Hands Rotation
            float bigZ = Mathf.Lerp(startBigZ, endBigZ, t);
            float smallZ = Mathf.Lerp(startSmallZ, endSmallZ, t);

            clockBigHand.rotation = Quaternion.Euler(0f, bigHandYRotation + 45f, bigZ);
            clockSmallHand.rotation = Quaternion.Euler(0f, smallHandYRotation + 45f, smallZ);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Final precise rotations
        transform.rotation = Quaternion.Euler(0f, valveYRotation + 45f, endValveZ % 360f);
        clockBigHand.rotation = Quaternion.Euler(0f, bigHandYRotation + 45f, endBigZ % 360f);
        clockSmallHand.rotation = Quaternion.Euler(0f, smallHandYRotation + 45f, endSmallZ % 360f);


        CheckPassword();
        isRotating = false;
    }

    private void CheckPassword()
    {
        float bigZ = clockBigHand.eulerAngles.z % 360f;
        float smallZ = clockSmallHand.eulerAngles.z % 360f;

        float expectedBigZ = 180f;
        float expectedSmallZ = 135f;
        float tolerance = 1f;

        bool isBigCorrect = Mathf.Abs(bigZ - expectedBigZ) <= tolerance;
        bool isSmallCorrect = Mathf.Abs(smallZ - expectedSmallZ) <= tolerance;

        if (isBigCorrect && isSmallCorrect)
        {
            Debug.Log("Password Correct!");
            EvntManager.TriggerEvent("SetToken");
            tL.TeaClock = true;

        }
        else
        {
            Debug.Log("Password Incorrect.");
            EvntManager.TriggerEvent("SetTea");
            tL.TeaClock = false;
        }
    }


}
