using UnityEngine;
using DG.Tweening; 
using UnityEngine.UI; 
using TMPro; 

public class StartScreen : MonoBehaviour
{
    [SerializeField] private CanvasGroup startScreenCanvasGroup; 
    [SerializeField] private TextMeshProUGUI pressAnyKeyText; 
    [SerializeField] private float fadeDuration = 1f; 

    private bool isGameStarted = false;

    void Start()
    {
        startScreenCanvasGroup.alpha = 1f;
        pressAnyKeyText.text = "Press any key to start the game.";
    }

    void Update()
    {
        if (!isGameStarted && Input.anyKeyDown)
        {
            StartGame();
        }
    }

    void StartGame()
    {
        isGameStarted = true;

        startScreenCanvasGroup.DOFade(0f, fadeDuration).OnComplete(() =>
        {
            startScreenCanvasGroup.gameObject.SetActive(false);
            Debug.Log("Oyun başladı!");
            Destroy(gameObject);
        });
    }

}