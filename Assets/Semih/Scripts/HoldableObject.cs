using UnityEngine;
public class HoldableObject : InteractableObject
{
    [SerializeField] private bool isHeld = false;
    public bool IsHeld;
    public virtual void PickUp(FirstPersonController controller) => controller.PickUp();
    public virtual void Drop(FirstPersonController controller) => controller.Drop();
    public virtual void Rotate(FirstPersonController controller, float angle) => controller.RotateHeldObject(Vector2.zero);

    public override void Interact(FirstPersonController controller)
    {
        if (!IsHeld)
            PickUp(controller);
    }
}
