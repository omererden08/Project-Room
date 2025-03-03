using UnityEngine;

public class IInteractable : MonoBehaviour
{
    public void Interact() { Debug.Log("Interacted with " + gameObject.name); }
}
