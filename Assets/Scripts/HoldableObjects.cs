using UnityEngine;

public class HoldableObjects : IInteractable
{
    public override void Interact()
    {
        base.Interact();
        
    }

    public void Drop()
    {
        Debug.Log("Dropped " + gameObject.name);
    }
    public void Rotate()
    {
        Debug.Log("Rotated " + gameObject.name);
    }
}
