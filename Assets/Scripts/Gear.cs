using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class Gear : MonoBehaviour
{
    public Transform FirstPosition;
    public Slot targetSlot;
    
    private bool isDragging = false;
    private Vector3 offset;
    private float zCoordinate;
    private Slot currentSlot; // Private olarak kalıyor
    private GearPuzzle gearPuzzle;

    public UnityEvent action;

    // Getter ekledik
    public Slot CurrentSlot => currentSlot;

    void Start()
    {
        FirstPosition = transform;
        // Eski metod: FindObjectOfType
        gearPuzzle = UnityEngine.Object.FindObjectOfType<GearPuzzle>();
    }

    void OnMouseDown()
    {
        zCoordinate = Camera.main.WorldToScreenPoint(transform.position).z;
        offset = transform.position - GetMouseWorldPosition();
        isDragging = true;
        
        if (currentSlot != null)
        {
            currentSlot.isOccupied = false;
            currentSlot.isCorrect = false;
            currentSlot = null;
        }
        
        transform.DOKill(); // Dönmeyi durdur
    }

    void OnMouseUp()
    {
        isDragging = false;
        
        Slot nearestSlot = FindNearestSlot();
        
        if (nearestSlot != null && !nearestSlot.isOccupied)
        {
            MoveToPosition(nearestSlot.transform);
            currentSlot = nearestSlot;
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
            
            if (slot == targetSlot)
            {
                slot.isOccupied = true;
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
                slot.isOccupied = true;
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
        if (currentSlot != null && currentSlot == targetSlot)
        {
            int slotIndex = System.Array.IndexOf(gearPuzzle.slots, currentSlot);
            if (gearPuzzle.CanGearSpin(slotIndex))
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
                    transform.DOKill(); // Dönmeyi durdur
                    currentSlot.isCorrect = false;
                }
            }
        }
    }

    private void StartSpinning()
    {
        transform.DOKill();
        transform.DORotate(new Vector3(0, 360, 0), 1f, RotateMode.LocalAxisAdd)
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
        // Eski metod: FindObjectsOfType
        Slot[] allSlots = UnityEngine.Object.FindObjectsOfType<Slot>();
        Slot nearestSlot = null;
        float minDistance = float.MaxValue;
        float snapDistance = 0.3f;

        foreach (Slot slot in allSlots)
        {
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