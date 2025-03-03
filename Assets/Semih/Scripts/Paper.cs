using System.Collections.Generic;
using UnityEngine;

public class Paper : HoldableObject
{
    [SerializeField] private string textContent = "Default paper text";

    public string Read() => textContent;

    public override void Interact(FirstPersonController controller)
    {
        if (!IsHeld)
            base.Interact(controller);
        else
            Debug.Log($"Reading: {Read()}");
    }
}
