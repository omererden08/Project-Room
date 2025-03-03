using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    [SerializeField] protected bool outlineEnabled = true;
    public bool IsInteractable => outlineEnabled && gameObject.activeInHierarchy;

    public abstract void Interact(FirstPersonController controller);
}
