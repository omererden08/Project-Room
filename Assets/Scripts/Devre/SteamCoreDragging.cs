using UnityEngine;
using DG.Tweening;

public class SteamCoreDragging : MonoBehaviour
{
    public PuzzleManager puzzleManager;
    public DevreSystem devreSystem;

    private bool isDragging;
    private Vector3 dragOffset;
    private Vector3 initialPosition;

    void Start()
    {
        devreSystem = FindObjectOfType<DevreSystem>();
        puzzleManager = devreSystem.puzzleManager;
        transform.rotation = Quaternion.Euler(90, 0, -90);
    }


    private void OnMouseDown()
    {
        if (!puzzleManager.inPuzzleMode) return;

        isDragging = true;
        initialPosition = transform.position;

        // Calculate the offset between mouse position and object position
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        dragOffset = transform.position - mouseWorldPos;
    }

    private void OnMouseDrag()
    {
        if (!puzzleManager.inPuzzleMode || !isDragging) return;

        Vector3 mouseWorldPos = GetMouseWorldPosition();
        Vector3 newPosition = mouseWorldPos + dragOffset;

        // Maintain the original X position if you only want vertical dragging
        // Remove this line if you want full 2D dragging
        // newPosition.x = initialPosition.x;

        transform.position = newPosition;
    }

    private void OnMouseUp()
    {
        if (!puzzleManager.inPuzzleMode) return;

        isDragging = false;

        // Check if this object is overlapping with any CoreSlotDisk objects
        CoreSlotDisk[] slots = FindObjectsOfType<CoreSlotDisk>();
        foreach (CoreSlotDisk slot in slots)
        {
            if (IsOverlapping(slot.gameObject))
            {
                // Snap to the position of the CoreSlotDisk
                transform.position = slot.transform.position;
                break; // Stop after finding the first overlapping slot
            }
        }
    }
    private bool IsOverlapping(GameObject other)
    {
        // Get the colliders
        Collider thisCollider = GetComponent<Collider>();
        Collider otherCollider = other.GetComponent<Collider>();

        // Check if both objects have colliders
        if (thisCollider == null || otherCollider == null)
        {
            Debug.LogWarning("Missing collider on one of the objects to check overlap");
            return false;
        }

        // Check for overlap between the two colliders
        return thisCollider.bounds.Intersects(otherCollider.bounds);
    }
    private Vector3 GetMouseWorldPosition()
    {
        // Convert mouse position to world position at the same Z-depth as the object
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }
}