using UnityEngine;
using System.Linq;

public class GearPuzzle : MonoBehaviour
{
    [SerializeField] private Slot[] slots;
    [SerializeField] private PuzzleManager puzzleManager;
    private int requiredCorrectSlots;

    private void Awake()
    {
        requiredCorrectSlots = slots.Count();
        //Debug.Log($"GearPuzzle: Required correct slots = {requiredCorrectSlots}, Total slots = {slots.Length}");
    }

    public void UpdateGearStates()
    {
        bool canSpin = true;

        for (int i = 0; i < slots.Length; i++)
        {
            if (i == 4)
            {
                if (slots[1].currentStat == SlotStat.IncorrectAndFake)
                {
                    canSpin = false;
                    continue;
                }
            }

            Slot slot = slots[i];
            Gear gear = slot.CurrentGear;

            bool isCorrect = slot.CurrentStat == SlotStat.Correct ||
                            slot.CurrentStat == SlotStat.CorrectAndFake ||
                            slot.CurrentStat == SlotStat.IncorrectAndFake;


            if (!isCorrect)
            {
                canSpin = false;
            }

            if (gear != null)
            {
                gear.UpdateSpinning(canSpin && isCorrect, slot.clockwise);
            }

            if (!canSpin)
            {
                for (int j = i + 1; j < slots.Length; j++)
                {
                    if (slots[j].CurrentGear != null)
                    {
                        slots[j].CurrentGear.UpdateSpinning(false, slots[j].clockwise);
                    }
                }
                break;
            }

        }


        CheckPuzzleCompletion();
    }

    private void CheckPuzzleCompletion()
    {
        int correctCount = slots.Count(s =>
            s.CurrentStat == SlotStat.Correct &&
            s.CurrentGear != null || s.currentStat == SlotStat.CorrectAndFake); // Ensure slot is occupied

        Debug.Log($"Puzzle check: Correct slots = {correctCount}/{requiredCorrectSlots}");
        foreach (var slot in slots)
        {
            Debug.Log($"Slot {slot.name}: Fake={slot.isFakeSlot}, Stat={slot.CurrentStat}, Gear={(slot.CurrentGear != null ? slot.CurrentGear.name : "None")}");
        }

        if (correctCount >= requiredCorrectSlots)
        {
            Debug.Log("Puzzle Solved!");
            puzzleManager.PuzzleSolved();
        }
    }
}