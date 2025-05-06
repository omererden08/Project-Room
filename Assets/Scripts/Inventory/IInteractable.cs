using UnityEngine;
using UnityEngine.Rendering;

public class IInteractable : MonoBehaviour
{
    public Item item;
    public Outline3D outline;

    void Start()
    {
        if (outline == null)
            outline = GetComponent<Outline3D>();
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
        // Add to inventory and deactivate the object
        Inventory inventory = FindObjectOfType<Inventory>();
        if (inventory != null)
        {
            if (inventory.AddItem(item, 1, gameObject))
            {
                gameObject.SetActive(false); // Deactivate instead of destroying
            }
        }
    }

    public virtual void Interact()
    {
        Debug.Log("Interacted with " + gameObject.name);
    }
}