using UnityEngine;
using DG.Tweening;

public enum GearSize
{
    Medium,
    Large
}

public class Gear : MonoBehaviour
{
    public Transform firstPosition;
    public GearSize size;

    private bool isDragging;
    private Vector3 offset;
    private float zCoordinate;
    private Slot currentSlot;
    private GearPuzzle gearPuzzle;
    private PuzzleManager puzzleManager;

    public Slot CurrentSlot => currentSlot;
    public bool IsSpinning => transform.GetComponent<Tween>()?.IsActive() ?? false;

    private void Awake()
    {
        firstPosition = transform;
        gearPuzzle = FindFirstObjectByType<GearPuzzle>();
        puzzleManager = FindFirstObjectByType<PuzzleManager>();
        //Debug.Log($"Gear {name} initialized: Size={size}");
    }

    private void OnMouseDown()
    {
        if (!puzzleManager.inPuzzleMode) return;

        zCoordinate = Camera.main.WorldToScreenPoint(transform.position).z;
        offset = transform.position - GetMouseWorldPosition();
        isDragging = true;

        if (currentSlot != null)
        {
            currentSlot.ClearGear();
            currentSlot = null;
        }

        transform.DOKill();
    }

    private void OnMouseUp()
    {
        if (!puzzleManager.inPuzzleMode) return;

        isDragging = false;
        Slot nearestSlot = FindNearestSlot();

        if (nearestSlot != null && !nearestSlot.IsOccupied)
        {
            MoveToSlot(nearestSlot);
        }
        else
        {
            MoveToFirstPosition();
        }
    }

    private void Update()
    {
        if (isDragging)
        {
            transform.position = GetMouseWorldPosition() + offset;
        }
    }

    public void MoveToSlot(Slot slot)
    {
        currentSlot = slot;
        transform.DOMove(slot.transform.position, 0.5f).OnComplete(() =>
        {
            Debug.Log($"Gear {name} moved to slot {slot.name}");
            slot.ValidateGear(this);
        });
    }

    public void MoveToFirstPosition()
    {
        transform.DOMove(firstPosition.position, 0.5f);
        currentSlot = null;
        Debug.Log($"Gear {name} returned to first position");
    }

    public void UpdateSpinning(bool shouldSpin, bool clockwise)
    {
        transform.DOKill();

        if (shouldSpin)
        {
            transform.DORotate(new Vector3(0, clockwise ? 360 : -360, 0), 5f, RotateMode.LocalAxisAdd)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart)
                .SetRelative();
        }
        Debug.Log($"Gear {name}: Spinning={shouldSpin}, Clockwise={clockwise}");
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = zCoordinate;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    private Slot FindNearestSlot()
    {
        Slot[] allSlots = FindObjectsByType<Slot>(FindObjectsSortMode.None);
        Slot nearestSlot = null;
        float minDistance = float.MaxValue;
        const float snapDistance = 0.1f;

        foreach (Slot slot in allSlots)
        {
            if (slot.IsOccupied) continue;

            float distance = Vector3.Distance(transform.position, slot.transform.position);
            if (distance < minDistance && distance < snapDistance)
            {
                minDistance = distance;
                nearestSlot = slot;
            }
        }

        return nearestSlot;
    }
}