using UnityEngine;
using System.Linq;

public class GearPuzzle : MonoBehaviour
{
    public Slot[] slots;
    [SerializeField] private int requiredCorrectSlots;

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
        }
        else
        {
            Debug.Log($"Puzzle not completed! {correctCount}/{requiredCorrectSlots} correct");
        }
    }

    public bool CanGearSpin(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slots.Length) return false;

        Slot currentSlot = slots[slotIndex];
        
        if (currentSlot.isFakeSlot)
        {
            return currentSlot.currentGear != null && 
                   currentSlot.acceptedSizes.Contains(currentSlot.currentGear.size);
        }

        if (slotIndex == 0) 
            return currentSlot.currentGear != null && 
                   currentSlot.acceptedSizes.Contains(currentSlot.currentGear.size);

        for (int i = 0; i < slotIndex; i++)
        {
            if (slots[i].isFakeSlot) continue;
            
            if (slots[i].currentGear == null || 
                !slots[i].acceptedSizes.Contains(slots[i].currentGear.size))
            {
                return false;
            }
        }
        
        return currentSlot.currentGear != null && 
               currentSlot.acceptedSizes.Contains(currentSlot.currentGear.size);
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
            }
        }

        foreach (Gear gear in allGears)
        {
            if (gear.CurrentSlot != null)
            {
                gear.CheckAndUpdateSpinning();
            }
        }

        CheckPuzzleCompletion();
    }
}