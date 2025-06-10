using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
public class UIPaper : MonoBehaviour
{
    public RectTransform paper;
    public RectTransform targetPositions;
    public Image paperImage;
    public RectTransform closedPosition;
    private bool isActive;

    void Start()
    {
        paperImage = GetComponent<Image>();
        paper.anchoredPosition = closedPosition.anchoredPosition;
        isActive = false;
    }
    public void WritePaper()
    {
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
        if (isActive && Input.GetMouseButton(1))
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
