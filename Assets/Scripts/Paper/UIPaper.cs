using UnityEngine;
using TMPro;
using DG.Tweening;
public class UIPaper : MonoBehaviour
{
    public RectTransform paper;
    public RectTransform targetPositions;
    public TextMeshProUGUI contentText;
    public RectTransform closedPosition;
    private bool isActive;

    void Start()
    {
        paper.anchoredPosition = closedPosition.anchoredPosition;
        isActive = false;
    }
    public void WritePaper(string content)
    {
        contentText.text = content;
        OpenPaper();
    }
    public void OpenPaper()
    {
        paper.DOAnchorPos(targetPositions.anchoredPosition, 0.5f).SetEase(Ease.OutBack);
        isActive = true;
        Debug.Log("Opening paper");
    }
    void Update()
    {
        if (isActive && Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePaper();
        }
    }
    public void ClosePaper()
    {
        paper.DOAnchorPos(closedPosition.anchoredPosition, 0.5f).SetEase(Ease.OutBack);
        isActive = false;
        Debug.Log("Closing paper");
    }
}
