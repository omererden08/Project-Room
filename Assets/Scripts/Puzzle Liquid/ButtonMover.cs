using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class ButtonMover : MonoBehaviour
{
    public enum ButtonType
    {
        Toggle,   
        OneShot   
    }
    [SerializeField] private UnityEvent unityAction;

    [Header("Movement Settings")]
    [SerializeField] private Transform targetPos;
    [SerializeField] private float moveDuration = 0.5f;

    [Header("Button Behavior")]
    [SerializeField] private ButtonType buttonType = ButtonType.Toggle;

    private Vector3 initialPos;
    private Coroutine moveCoroutine;
    private bool isMoving = false;
    private bool isOpen = false;

    void Start()
    {
        initialPos = transform.position;

        if (targetPos == null && transform.childCount > 0)
        {
            targetPos = transform.GetChild(0);
        }

        if (targetPos == null)
        {
            Debug.LogError($"TargetPos is null on {gameObject.name}");
        }
    }

    public void OnMouseDown()
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

    // Public method to move to target position
    public void MoveToTarget()
    {
        if (isMoving || isOpen)
        {
            Debug.Log($"MoveToTarget skipped on {gameObject.name}: isMoving={isMoving}, isOpen={isOpen}");
            return;
        }

        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        moveCoroutine = StartCoroutine(Move(true, moveDuration));
    }

    // Public method to move to initial position
    public void MoveToInitial()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }

        // Force reset if not already at initial position
        if (isOpen || isMoving)
        {
            isMoving = true;
            moveCoroutine = StartCoroutine(Move(false, moveDuration));
            Debug.Log($"Moving {gameObject.name} to initial position");
        }
        else
        {
            Debug.Log($"MoveToInitial skipped on {gameObject.name}: Already at initial position");
        }
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
        AreYouWinningDad();
        isOpen = open;
        isMoving = false;
        moveCoroutine = null;
        Debug.Log($"{gameObject.name} moved to {(open ? "target" : "initial")} position");
    }

    private IEnumerator OneShotMove(float duration)
    {
        isMoving = true;

        Vector3 start = initialPos;
        Vector3 end = targetPos.position;

        float elapsedTime = 0f;

        // Move to target
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.position = Vector3.Lerp(start, end, t);
            elapsedTime += Time.deltaTime;
            AreYouWinningDad();
            yield return null;
        }

        transform.position = end;

        yield return new WaitForSeconds(0.2f);

        // Move back
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
        Debug.Log($"{gameObject.name} completed OneShot move");
    }

    public void AreYouWinningDad()
    {
        unityAction?.Invoke();
    }
}