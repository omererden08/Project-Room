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
        audioSource.Play();
        FadeManager.Instance.FadeBlack("Intro");
    }

    public void ReturnMenu()
    {
        audioSource.Play();
        FadeManager.Instance.FadeBlack("MainMenu");
    }
    public void Restart()
    {
        audioSource.Play();
        FadeManager.Instance.FadeBlack("Gameplay");
    }
    public void Quit()
    {
        audioSource.Play();
        FadeManager.Instance.Quit();
    }
}
