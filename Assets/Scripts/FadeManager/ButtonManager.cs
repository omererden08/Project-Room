using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    private AudioSource audioSource;
    private Image howtoPlay;
    private bool isOpened = false;


    public static ButtonManager Instance { get; private set; }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        howtoPlay = GameObject.Find("HowToPlay").GetComponent<Image>();
        howtoPlay.gameObject.SetActive(false);
    }

    void Update()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
       
    }

    public void StartGame()
    {
        audioSource.Play();
        if (isOpened)
        {
            howtoPlay.gameObject.SetActive(false);
            isOpened = false;
        }

        FadeManager.Instance.FadeToScene("Gameplay");
    }

    public void ReturnMenu()
    {
        audioSource.Play();
        FadeManager.Instance.FadeToScene("MainMenu");
    }
    public void Restart()
    {
        audioSource.Play();
        FadeManager.Instance.FadeToScene("Gameplay");
    }
    public void Quit()
    {
        audioSource.Play();
        FadeManager.Instance.Quit();
    }
}
