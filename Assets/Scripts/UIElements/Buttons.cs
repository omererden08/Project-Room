using UnityEngine;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime;

public class Buttons : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private GameObject gear1;
    [SerializeField] private GameObject gear2;
    [SerializeField] private GameObject door;
    private Animator gear1Animator;
    private Animator gear2Animator;
    private Vector3 initialPos;
    private bool isMoving = false;
    [SerializeField] private float moveDuration;

    private bool isHovered;

    public enum ButtonType
    {
        Start,
        Quit
    }
    [SerializeField] private ButtonType buttonType;


    private void Start()
    {
        initialPos = transform.position;

        if (target == null && transform.childCount > 0)
        {
            target = transform.GetChild(0);
        }
        gear1Animator = gear1.GetComponent<Animator>();
        gear2Animator = gear2.GetComponent<Animator>();
    }


    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform == this.transform)
            {
                if (!isHovered)
                {
                    isHovered = true;

                    switch (buttonType)
                    {
                        case ButtonType.Start:
                            gear1Animator.SetBool("start", true);
                            gear2Animator.SetBool("quit", false);
                            break;

                        case ButtonType.Quit:
                            gear2Animator.SetBool("quit", true);
                            gear1Animator.SetBool("start", false);
                            break;
                    }

                    Debug.Log("Hover ba�lad�");
                }

                if (Input.GetMouseButtonDown(0) && !isMoving)
                {
                    OnButtonPressed();
                }
            }
            else
            {
                if (isHovered)
                {
                    isHovered = false;
                    ResetAnimatorStates();
                }
            }
        }
        else
        {
            if (isHovered)
            {
                isHovered = false;
                ResetAnimatorStates();
            }
        }
    }

    private void ResetAnimatorStates()
    {
        gear1Animator.SetBool("start", false);
        gear2Animator.SetBool("quit", false);
    }

    private void OnButtonPressed()
    {
        if (isMoving)
            return;

        StartCoroutine(OneShotMove(moveDuration));

        switch (buttonType)
        {
            case ButtonType.Start:
                door.GetComponent<Animator>().SetTrigger("Door_Unlock_All");
                FadeManager.Instance.FadeBlackDelayed("Intro");
                break;

            case ButtonType.Quit:
                FadeManager.Instance.Quit();
                break;
        }
    }

    private IEnumerator OneShotMove(float duration)
    {
        isMoving = true;

        Vector3 start = initialPos;
        Vector3 end = target.position;

        float elapsedTime = 0f;

        // Gidi�
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.position = Vector3.Lerp(start, end, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = end;

        yield return new WaitForSeconds(0.2f);

        // D�n��
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
    }

    



}
