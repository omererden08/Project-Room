using UnityEngine;
using UnityEngine.Rendering;
public class IInteractable : MonoBehaviour
{
    public Item item;
    public Outline3D outline;
    void Start()
    {
        if (outline == null)
        {
            Debug.Log("Outline component is missing on " + gameObject.name);
            outline = GetComponent<Outline3D>();
        }
       
        outline.enabled = false;
    }
    public virtual void OutlineShow()
    {
        outline.enabled = true;
        Debug.Log(gameObject.name + " outline show");
    }
    public virtual void OutlineHide()
    {
        outline.enabled = false;
        Debug.Log(gameObject.name + " outline hide");
    }
    public virtual void PickUp()
    {
        Debug.Log("Picked up " + gameObject.name);
    }
    public virtual void Interact() { Debug.Log("Interacted with " + gameObject.name); }
}
