using UnityEngine;

public class IInteractable : MonoBehaviour
{
    public Item item;
    public virtual void OutlineShow()
    {
        Debug.Log(gameObject.name + " outline show");
    }
    public virtual void OutlineHide()
    {
        Debug.Log(gameObject.name + " outline hide");
    }
    public virtual void PickUp()
    {
        Debug.Log("Picked up " + gameObject.name);
    }
    public virtual void Interact() { Debug.Log("Interacted with " + gameObject.name); }
}
