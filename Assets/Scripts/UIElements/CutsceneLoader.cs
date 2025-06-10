using UnityEngine;
using UnityEngine.UI;

public class CutsceneLoader : MonoBehaviour
{
    [SerializeField] private Sprite[] cutsceneSprites;  // Tüm cutscene görselleri
    [SerializeField] private Image imageRenderer;       // UI Image bileþeni

    private int spriteIndex = 0;
    private bool isFinished = false;

    private void Start()
    {
        // imageRenderer sahneden atanmadýysa, bu nesnedeki Image bileþenini al
        if (imageRenderer == null)
            imageRenderer = GetComponent<Image>();

        Cursor.visible = false;

        // Ýlk sprite’ý göster
        if (cutsceneSprites != null && cutsceneSprites.Length > 0)
        {
            imageRenderer.sprite = cutsceneSprites[spriteIndex];
        }
        else
        {
            Debug.LogWarning("Cutscene sprites are not assigned.");
            isFinished = true;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isFinished)
        {
            NextCutScene();
        }
    }

    private void NextCutScene()
    {
        spriteIndex++;

        if (spriteIndex >= cutsceneSprites.Length)
        {
            isFinished = true;
            FadeManager.Instance.FadeToNextScene();
            return;
        }

        imageRenderer.sprite = cutsceneSprites[spriteIndex];
    }
}
