using System.Collections.Generic;
using UnityEngine;

public class MergeableObject : HoldableObject
{
    [SerializeField] private List<MergeableObject> compatibleObjects = new List<MergeableObject>();

    public bool CanMergeWith(MergeableObject other) => compatibleObjects.Contains(other);

    public void Merge(MergeableObject other)
    {
        if (!CanMergeWith(other)) return;

        Debug.Log($"Merging {gameObject.name} with {other.gameObject.name}");
        // Example merging logic: Combine properties, destroy one object, etc.
        GameObject mergedObject = new GameObject("MergedObject");
        mergedObject.transform.position = transform.position;
        Destroy(gameObject); // Destroy this object
        Destroy(other.gameObject); // Destroy the other object
        // You can create a new merged prefab or modify properties here
    }

    public override void Interact(FirstPersonController controller)
    {
        if (!IsHeld)
            base.Interact(controller);
    }
}