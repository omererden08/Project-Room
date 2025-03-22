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
    private Slot currentSlot;

    public UnityEvent action;
    void Start()
    {
        FirstPosition = transform;
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
        });
    }

    private void CheckPosition(Transform targetPosition)
    {
        Slot slot = targetPosition.GetComponent<Slot>();
        if (slot != null && !slot.isOccupied)
        {
            if (slot == targetSlot)
            {
                StartSpinning();
                slot.isOccupied = true;
                slot.isCorrect = true;
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
        Slot[] allSlots = FindObjectsOfType<Slot>();
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