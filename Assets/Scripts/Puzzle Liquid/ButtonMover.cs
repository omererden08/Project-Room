using UnityEngine;
using System.Collections;

public class ButtonMover : MonoBehaviour
{
    public enum ButtonType
    {
        Toggle,   // Aç-kapa
        OneShot   // Açýlýr, sonra otomatik geri döner
    }

    [Header("Movement Settings")]
    [SerializeField] private Transform targetPos;
    [SerializeField] private float moveDuration = 0.5f;

    [Header("Button Behavior")]
    [SerializeField] private ButtonType buttonType = ButtonType.Toggle;

    private Vector3 initialPos;
    private Coroutine moveCoroutine;
    private bool isMoving = false;
    private bool isOpen = false; // Sadece Toggle tipi için kullanýlýr

    void Start()
    {
        initialPos = transform.position;

        if (targetPos == null && transform.childCount > 0)
        {
            targetPos = transform.GetChild(0);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.transform == transform)
            {
                OnButtonPressed();
            }
        }
    }

    private void OnButtonPressed()
    {
        if (isMoving)
            return;

        switch (buttonType)
        {
            case ButtonType.Toggle:
                ToggleMove();
                break;

            case ButtonType.OneShot:
                if (moveCoroutine != null)
                    StopCoroutine(moveCoroutine);
                moveCoroutine = StartCoroutine(OneShotMove(moveDuration));
                break;
        }
    }

    private void ToggleMove()
    {
        bool goingOpen = !isOpen;

        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        moveCoroutine = StartCoroutine(Move(goingOpen, moveDuration));
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
        isOpen = open;
        isMoving = false;
        moveCoroutine = null;
    }

    private IEnumerator OneShotMove(float duration)
    {
        isMoving = true;

        Vector3 start = initialPos;
        Vector3 end = targetPos.position;

        float elapsedTime = 0f;

        // Gidiþ
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.position = Vector3.Lerp(start, end, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = end;

        yield return new WaitForSeconds(0.2f);

        // Dönüþ
        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.position = Vector3.Lerp(end, start, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = start;

        isMoving = false;
        moveCoroutine = null;
    }
}
