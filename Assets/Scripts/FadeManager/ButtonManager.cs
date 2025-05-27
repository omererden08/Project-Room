using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    private AudioSource audioSource;
    private bool isOpened = false;


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }


    public void StartGame()
    {
        Time.timeScale = 1f;
        audioSource.Play();
        FadeManager.Instance.FadeBlackDelayed("Intro");
    }

    public void ReturnMenu()
    {
        Time.timeScale = 1f;

        audioSource.Play();
        FadeManager.Instance.FadeBlack("MainMenu");
    }
    public void Restart()
    {
        Time.timeScale = 1f;

        audioSource.Play();
        FadeManager.Instance.FadeBlack("Gameplay");
    }
    public void Quit()
    {
        Time.timeScale = 1f;

        audioSource.Play();
        FadeManager.Instance.Quit();

    }
}
