using System.Collections;
using UnityEngine;

public class BombAnimation : IInteractable
{
    [Header("Movement")]
    [SerializeField] private Transform targetPos;
    [SerializeField] private GameObject keyObject;
    private Animator keyAnimator;
    private float moveDuration = 0.55f;
    private float platformDuration = 4f;
    private float delayTime; // Delay for bomb after platform

    private Vector3 initialPos;
    [SerializeField] private bool isMoving = false;
    private bool isOpen = false;

    void Start()
    {
        outline = GetComponent<Outline3D>();
        EvntManager.StartListening("BombUpStart", BombUpStart);
        initialPos = transform.position;

        if (targetPos == null && transform.childCount > 0)
        {
            targetPos = transform.GetChild(0);
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
