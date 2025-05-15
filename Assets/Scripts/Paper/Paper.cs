using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class Paper : IInteractable
{
    public UIPaper uiPaper;
    public PaperData paperData;

    void Start()
    {
        outline = GetComponent<Outline3D>();
        outline.enabled = false;
    }
    public override void Interact()
    {
        Image paperImage = uiPaper.GetComponent<Image>();
        paperImage.sprite = paperData.paperImage;
        uiPaper.WritePaper();
        base.Interact();
    }

}
