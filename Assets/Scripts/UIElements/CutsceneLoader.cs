using UnityEngine;
using UnityEngine.UI;

public class CutsceneLoader : MonoBehaviour
{
    [SerializeField] private Sprite[] cutsceneSprites;
    [SerializeField] private Image imageRenderer;

    private int spriteIndex = 0;

    private void Start()
    {
        imageRenderer = GetComponent<Image>();
        Cursor.visible = false;

        // Ýlk sprite’ý göster
        if (cutsceneSprites.Length > 0)
        {
            imageRenderer.sprite = cutsceneSprites[spriteIndex];
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            NextCutScene();
        }
    }

    void NextCutScene()
    {
        spriteIndex++;

        if (spriteIndex >= cutsceneSprites.Length)
        {
            FadeManager.Instance.FadeToNextScene();
            return;
        }

        imageRenderer.sprite = cutsceneSprites[spriteIndex];
    }
}
