using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;

    [SerializeField] private CanvasGroup blackFadeGroup;
    [SerializeField] private CanvasGroup whiteFadeGroup;
    [SerializeField] private float blackFadeDuration;
    [SerializeField] private float whiteFadeDuration;
    [SerializeField] private float delayTime;

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
        whiteFadeGroup.alpha = 0;
        blackFadeGroup.alpha = 1;
        blackFadeGroup.DOFade(0, blackFadeDuration).OnComplete(() =>
        {
            this.gameObject.SetActive(false);
        });

    }



    public void BlackScene(string sceneName)
    {
        gameObject.SetActive(true);

        blackFadeGroup.DOFade(1, 0f).SetUpdate(true).OnComplete(() =>
        {
            // Sahneyi yüklüyoruz, sonra bekleyip açacağız
            SceneManager.LoadScene(sceneName);

            StartCoroutine(FadeInAfterDelay(blackFadeGroup, blackFadeDuration));
        });
    }


    public void FadeBlack(string sceneName)
    {
        gameObject.SetActive(true);

        float delayBeforeFade = delayTime; // Örneğin 0.5 saniye gecikme

        // Delay’den sonra fade başlasın
        DOVirtual.DelayedCall(delayBeforeFade, () =>
        {
            blackFadeGroup.DOFade(1, blackFadeDuration).SetUpdate(true).OnComplete(() =>
            {
                SceneManager.LoadScene(sceneName);
                StartCoroutine(FadeInAfterDelay(blackFadeGroup, blackFadeDuration));
            });
        }).SetUpdate(true);
    }

    public void FadeWhite(string sceneName)
    {
        gameObject.SetActive(true);
        whiteFadeGroup.alpha = 0;

        whiteFadeGroup.DOFade(0, whiteFadeDuration).SetUpdate(true).OnComplete(() =>
        {
            // Sahneyi yüklüyoruz, sonra bekleyip açacağız
            SceneManager.LoadScene(sceneName);

            StartCoroutine(FadeInAfterDelay(whiteFadeGroup, whiteFadeDuration));
        });
    }

    private IEnumerator FadeInAfterDelay(CanvasGroup canvasGroup, float duration)
    {
        // 1 saniyelik cooldown (bekleme)
        yield return new WaitForSeconds(1f);

        canvasGroup.DOFade(0, duration).SetUpdate(true).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }

    public void Quit()
    {
        this.gameObject.SetActive(true);
        blackFadeGroup.DOFade(1, blackFadeDuration).OnComplete(() =>
        {
            Application.Quit();
        });
    }
}
