using System;
using UnityEngine;
using TMPro;
public class Paper : IInteractable
{
    public String paperName;
    public String content;
    public UIPaper uiPaper;

    void Start()
    {
        outline = GetComponent<Outline3D>();
        outline.enabled = false;
        uiPaper = FindObjectOfType<UIPaper>();
    }
    public override void Interact()
    {
        uiPaper.WritePaper(content);
        base.Interact();
    }

}
