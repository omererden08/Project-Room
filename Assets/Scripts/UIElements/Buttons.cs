using UnityEngine;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime;

public class Buttons : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private GameObject gear1;
    [SerializeField] private GameObject gear2;
    private Vector3 initialPos;
    private bool isMoving = false;
    [SerializeField] private float moveDuration;

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
    }


    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Hover kontrolü
            if (hit.transform == this.transform)
            {
                switch (buttonType)
                {
                    case ButtonType.Start:
                        FadeManager.Instance.FadeBlack("Gameplay 2");
                        break;

                    case ButtonType.Quit:
                        FadeManager.Instance.Quit();
                        break;
                }

                Debug.Log("Mouse, button Object'in üzerinde.");

                // Týklama kontrolü
                if (Input.GetMouseButtonDown(0) && !isMoving)
                {
                    OnButtonPressed();
                }
            }
        }
    }

    private void OnButtonPressed()
    {
        if (isMoving)
            return;

        StartCoroutine(OneShotMove(moveDuration));

        switch (buttonType)
        {
            case ButtonType.Start:
                FadeManager.Instance.FadeBlack("Gameplay 2");
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
    }

}
