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
        EvntManager.StartListening("BombUpStart", BombUpStart);
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


    public void BombUpStart()
    {
        ToggleDrawer(platformDuration);
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
        EvntManager.TriggerEvent("BombUp");
    }


}
