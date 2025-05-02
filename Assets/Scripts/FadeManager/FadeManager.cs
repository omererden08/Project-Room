using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;

    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeDuration = 1f;

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

    private void Start()
    {
        // Başlangıçta fade in
        fadeCanvasGroup.alpha = 1;
        fadeCanvasGroup.DOFade(0, fadeDuration).OnComplete(() =>
        {
            this.gameObject.SetActive(false);
        });

    }

    public void FadeToScene(string sceneName)
    {
        this.gameObject.SetActive(true);
        fadeCanvasGroup.DOFade(1, fadeDuration).SetUpdate(true).OnComplete(() =>
        {
            SceneManager.LoadScene(sceneName);
            // Yeni sahne yüklendiğinde tekrar fade in yap
            fadeCanvasGroup.DOFade(0, fadeDuration).SetUpdate(true).OnComplete(() =>
            {
                // Fade işlemi tamamlandığında bu nesneyi devre dışı bırak
                this.gameObject.SetActive(false);
            });
        });
    }
    public void Quit()
    {
        this.gameObject.SetActive(true);
        fadeCanvasGroup.DOFade(1, fadeDuration).OnComplete(() =>
        {
            Application.Quit();
        });
    }
}
