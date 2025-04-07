using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using System.Linq; // Contains metodu için

public enum GearSize
{
    Medium,
    Large
}

public class Gear : MonoBehaviour
{
    public Transform FirstPosition;
    public Slot targetSlot;
    public GearSize size;

    private bool isDragging = false;
    private Vector3 offset;
    private float zCoordinate;
    private Slot currentSlot;
    private GearPuzzle gearPuzzle;

    public UnityEvent action;
    [Tooltip("Dogru oldugundan emin ol")]
    public PuzzleManager puzzleManager;
    public Slot CurrentSlot => currentSlot;
    public bool IsSpinning => transform.GetComponent<Tween>()?.IsActive() ?? false;

    void Start()
    {
        FirstPosition = transform;
        gearPuzzle = UnityEngine.Object.FindFirstObjectByType<GearPuzzle>();
    }

    void OnMouseDown()
    {
        if (!puzzleManager.inPuzzleMode) return;

        zCoordinate = Camera.main.WorldToScreenPoint(transform.position).z;
        offset = transform.position - GetMouseWorldPosition();
        isDragging = true;

        if (currentSlot != null)
        {
            currentSlot.isOccupied = false;
            currentSlot.currentGear = null;
            currentSlot.isCorrect = false;
            currentSlot = null;
        }

        transform.DOKill();
    }

    void OnMouseUp()
    {
        if (!puzzleManager.inPuzzleMode) return;

        isDragging = false;

        Slot nearestSlot = FindNearestSlot();

        if (nearestSlot != null && !nearestSlot.isOccupied)
        {
            MoveToPosition(nearestSlot.transform);
            currentSlot = nearestSlot;
            currentSlot.currentGear = this;
        }
        else
        {
            MoveToPosition(FirstPosition);
        }
    }

    void Update()
    {
        if (isDragging)
        {
            transform.position = GetMouseWorldPosition() + offset;
        }
    }

    public void MoveToPosition(Transform targetPosition)
    {
        transform.DOMove(targetPosition.position, 0.5f).OnComplete(() =>
        {
            CheckPosition(targetPosition);
            gearPuzzle.UpdateAllGears();
        });
    }

    private void CheckPosition(Transform targetPosition)
    {
        Slot slot = targetPosition.GetComponent<Slot>();
        if (slot != null && !slot.isOccupied)
        {
            int slotIndex = System.Array.IndexOf(gearPuzzle.slots, slot);

            slot.isOccupied = true;
            slot.currentGear = this;

            if (slot.acceptedSizes.Contains(size))
            {
                if (gearPuzzle.CanGearSpin(slotIndex))
                {
                    StartSpinning();
                    slot.isCorrect = true;
                }
                else
                {
                    slot.isCorrect = false;
                }
            }
            else
            {
                slot.isCorrect = false;
            }
        }
        else if (targetPosition == FirstPosition)
        {
            // İlk pozisyona döndü
        }
        else
        {
            MoveToPosition(FirstPosition);
        }
        action.Invoke();
    }

    public void CheckAndUpdateSpinning()
    {
        if (currentSlot != null)
        {
            int slotIndex = System.Array.IndexOf(gearPuzzle.slots, currentSlot);

            if (currentSlot.acceptedSizes.Contains(size) &&
                gearPuzzle.CanGearSpin(slotIndex))
            {
                if (!currentSlot.isCorrect)
                {
                    StartSpinning();
                    currentSlot.isCorrect = true;
                }
            }
            else
            {
                if (currentSlot.isCorrect)
                {
                    transform.DOKill();
                    currentSlot.isCorrect = false;
                }
            }
        }
    }

    private void StartSpinning()
    {
        transform.DOKill();
        transform.DORotate(new Vector3(0, 360, 0), 5f, RotateMode.LocalAxisAdd)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart)
            .SetRelative();
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = zCoordinate;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    private Slot FindNearestSlot()
    {
        Slot[] allSlots = UnityEngine.Object.FindObjectsByType<Slot>(FindObjectsSortMode.None);
        Slot nearestSlot = null;
        float minDistance = float.MaxValue;
        float snapDistance = 0.3f;

        foreach (Slot slot in allSlots)
        {
            float distance = Vector3.Distance(transform.position, slot.transform.position);
            if (distance < minDistance && distance < snapDistance && !slot.isOccupied)
            {
                minDistance = distance;
                nearestSlot = slot;
            }
        }

        return nearestSlot;
    }
}