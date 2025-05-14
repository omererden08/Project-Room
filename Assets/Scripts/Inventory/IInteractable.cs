using JetBrains.Annotations;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.Rendering;

public class IInteractable : MonoBehaviour
{
    public Outline3D outline;
    public bool OutlineOki;
    public Bomb bomb;
    public bool isCollect;

    void Start()
    {

        if (outline == null)
            outline = GetComponent<Outline3D>();
        outline.enabled = false;
 
    }

    public virtual void OutlineShow()
    {
        if (!OutlineOki)
            return;
        outline.enabled = true;
    }

    public virtual void OutlineHide()
    {
        if (!OutlineOki)
            return;
        outline.enabled = false;
    }

    public virtual void PickUp()
    {
        if (!bomb.isBoombReady)
        {
            EvntManager.TriggerEvent("subID", "STRT_INTER_" + Random.Range(1, 5));
            return;
        }
    }

    public virtual void Interact()
    {
        Debug.Log("Interact");
        if(!isCollect)
        {
            EvntManager.TriggerEvent("subID", "STRT_INTER_" + Random.Range(1, 5));
            return;
        }
    }

}