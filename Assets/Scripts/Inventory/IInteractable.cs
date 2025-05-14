using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.Rendering;

public class IInteractable : MonoBehaviour
{
    public Outline3D outline;
    public bool OutlineOki;
    //buradaki outline ne kadar mantikkli acabana

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
    }

    public virtual void Interact()
    {
    }

}