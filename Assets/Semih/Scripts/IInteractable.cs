using UnityEngine;

public class IInteractable : MonoBehaviour
{
    public virtual void OutlineShow()
    {
        Debug.Log(gameObject.name + " outline show");
    }
    public virtual void OutlineHide()
    {
        Debug.Log(gameObject.name + " outline hide");
    }
    public virtual void Interact() { Debug.Log("Interacted with " + gameObject.name); }
}
