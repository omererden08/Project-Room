using System.Collections;
using UnityEngine;

public class Piston : MonoBehaviour
{

    [SerializeField] private GameObject indicatorObject;
    private Animator indicatorAnimator;
    private Animator pistonAnimator;
    [SerializeField] private float animDuration;

    private float animSpeed = 0f;
    private bool isOpen = false;

    void Start()
    {
        pistonAnimator = GetComponent<Animator>();
        indicatorAnimator = indicatorObject.GetComponent<Animator>();
        // 34. frame = 34 / 30 = 1.133 saniye (30fps animasyon varsayýmýyla)
        pistonAnimator.Play("PistonsOpen", 0, 34f / 30f);
        pistonAnimator.speed = 0f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isOpen)
        {
            StartCoroutine(StartPistonAnim());
            indicatorAnimator.SetBool("isOpen", true);
            isOpen = true;
        }

        if (Input.GetKeyDown(KeyCode.E) && isOpen)
        {
            StartCoroutine(StopPistonAnim());
            indicatorAnimator.SetBool("isOpen", false);
            isOpen = false;
        }
    }

    IEnumerator StartPistonAnim()
    {
        float elapsedTime = 0f;
        float startSpeed = 0f;
        float targetSpeed = 3.5f;

        while (elapsedTime < animDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / animDuration);
            animSpeed = Mathf.Lerp(startSpeed, targetSpeed, t); // Yavaþ yavaþ hýzlanma
            pistonAnimator.speed = animSpeed;
            yield return null;
        }

        pistonAnimator.speed = targetSpeed; // Sabit 5
        animSpeed = targetSpeed;
    }

    IEnumerator StopPistonAnim()
    {
        float elapsedTime = 0f;
        float startSpeed = animSpeed;
        float targetSpeed = 0f;

        while (elapsedTime < animDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / animDuration);
            animSpeed = Mathf.Lerp(startSpeed, targetSpeed, t); // Yavaþça yavaþla
            pistonAnimator.speed = animSpeed;
            yield return null;
        }

        pistonAnimator.speed = 0f; // Þu anki frame'de dursun
        animSpeed = 0f;
    }
}
