using UnityEngine;

public class NextScene : MonoBehaviour
{
    private bool canClick = false;
    private float fadeDuration = 3f; // Fade süresi
    private float timer = 0f; // Zamanlayýcý

    private void Start()
    {
        Cursor.visible = false;
    }

    private void Update()
    {
        // Zamanlayýcý çalýþýyor ve henüz süresi dolmadýysa
        if (!canClick)
        {
            timer += Time.deltaTime;
            if (timer >= fadeDuration)
            {
                canClick = true; // Artýk týklanabilir
            }
        }

        // Eðer týklanabilir durumdaysa ve sol týklama yapýlýrsa sahneyi deðiþtir
        if (canClick && Input.GetMouseButtonDown(0))
        {
            canClick = false; // Tek týklama olsun
            FadeManager.Instance.FadeToNextScene();
        }
    }
}
