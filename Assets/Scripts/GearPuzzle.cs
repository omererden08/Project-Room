using UnityEngine;
using System.Linq;
using DG.Tweening;

public class GearPuzzle : MonoBehaviour
{
    public Slot[] slots;
    [SerializeField] private int requiredCorrectSlots;
    public PuzzleManager puzzleManager;

    void Start()
    {
        if (slots == null || slots.Length == 0)
        {
            Debug.LogError("Slots dizisi boş veya null! Lütfen Inspector'dan doldurun.");
            return;
        }
        
        requiredCorrectSlots = slots.Count(s => !s.isFakeSlot);
    }

    public void CheckPuzzleCompletion()
    {
        if (slots == null || slots.Length == 0)
        {
            Debug.Log("Slots dizisi boş veya null!");
            return;
        }

        int correctCount = slots.Count(s => 
            !s.isFakeSlot && 
            s.currentGear != null && 
            s.acceptedSizes.Contains(s.currentGear.size));

        if (correctCount >= requiredCorrectSlots)
        {
            Debug.Log("Puzzle Completed!");
            puzzleManager.PuzzleSolved();
        }
        else
        {
            Debug.Log($"Puzzle not completed! {correctCount}/{requiredCorrectSlots} correct");
        }
    }

    public bool CanGearSpin(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slots.Length) 
        {
            Debug.Log($"Slot {slotIndex} out of bounds.");
            return false;
        }

        Slot currentSlot = slots[slotIndex];
        
        if (currentSlot.isFakeSlot)
        {
            bool canSpin = currentSlot.currentGear != null && 
                           currentSlot.acceptedSizes.Contains(currentSlot.currentGear.size);
            Debug.Log($"Slot {slotIndex} (fake): CanSpin = {canSpin}");
            return canSpin;
        }

        if (slotIndex == 0) 
        {
            bool canSpin = currentSlot.currentGear != null && 
                           currentSlot.acceptedSizes.Contains(currentSlot.currentGear.size);
            Debug.Log($"Slot {slotIndex} (first): CanSpin = {canSpin}");
            return canSpin;
        }

        for (int i = 0; i < slotIndex; i++)
        {
            if (slots[i].isFakeSlot) continue;
            
            if (slots[i].currentGear == null || 
                !slots[i].acceptedSizes.Contains(slots[i].currentGear.size))
            {
                Debug.Log($"Slot {slotIndex} blocked by slot {i}: Gear missing or wrong size.");
                return false;
            }
        }
        
        bool result = currentSlot.currentGear != null && 
                      currentSlot.acceptedSizes.Contains(currentSlot.currentGear.size);
        Debug.Log($"Slot {slotIndex}: CanSpin = {result}");
        return result;
    }

    public void UpdateAllGears()
    {
        Gear[] allGears = UnityEngine.Object.FindObjectsByType<Gear>(FindObjectsSortMode.None);
        if (allGears == null || allGears.Length == 0) return;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].currentGear != null)
            {
                slots[i].isCorrect = CanGearSpin(i);
                slots[i].currentGear.CheckAndUpdateSpinning();
            }
        }

        CheckPuzzleCompletion();
    }

    public void UpdateGearsBeforeIndex(int slotIndex)
    {
        for (int i = 0; i < slotIndex && i < slots.Length; i++)
        {
            if (slots[i].currentGear != null)
            {
                slots[i].isCorrect = CanGearSpin(i);
                slots[i].currentGear.CheckAndUpdateSpinning();
            }
        }
    }

    public void UpdateGearsAfterIndex(int slotIndex)
    {
        for (int i = slotIndex + 1; i < slots.Length; i++)
        {
            if (slots[i].currentGear != null)
            {
                slots[i].isCorrect = CanGearSpin(i);
                slots[i].currentGear.CheckAndUpdateSpinning();
            }
        }
    }

    public void StopSpinningAfterIndex(int slotIndex)
    {
        for (int i = slotIndex + 1; i < slots.Length; i++)
        {
            if (slots[i].currentGear != null)
            {
                slots[i].currentGear.transform.DOKill();
                slots[i].isCorrect = false;
            }
        }
    }
}