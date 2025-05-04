using UnityEngine;
using System.Linq;

public enum SlotStat
{
    Correct,
    CorrectAndFake,
    IncorrectAndFake,
    Incorrect
}

[System.Serializable]
public class Slot : MonoBehaviour
{
    public Gear[] correctGears;
    public bool clockwise;

    [SerializeField] private Gear currentGear;
    public bool isFakeSlot;
    public SlotStat currentStat;
    private GearPuzzle gearPuzzle;

    public Gear CurrentGear => currentGear;
    public SlotStat CurrentStat => currentStat;
    public bool IsOccupied => currentGear != null;

    private void Awake()
    {
        gearPuzzle = FindFirstObjectByType<GearPuzzle>();
        currentGear = null;
        currentStat = SlotStat.Incorrect; // Explicitly initialize
        //Debug.Log($"Slot {name} initialized: isFake={isFakeSlot}, stat={currentStat}");
    }

    public SlotStat ValidateGear(Gear gear)
    {
        currentGear = gear;
        currentStat = SlotStat.Incorrect;

        if (gear == null)
        {
            Debug.Log($"Slot {name}: Gear is null, stat={currentStat}");
            gearPuzzle.UpdateGearStates();
            return currentStat;
        }

        bool isCorrectGear = correctGears.Contains(gear);


        if (isCorrectGear )
        {
            currentStat = isFakeSlot ? SlotStat.CorrectAndFake : SlotStat.Correct;
        }
        else if (isFakeSlot)
        {
            currentStat = SlotStat.IncorrectAndFake;
        }

        Debug.Log($"Slot {name}: Gear={gear.name}, Size={gear.size}, CorrectGear={isCorrectGear}, Stat={currentStat}");
        gearPuzzle.UpdateGearStates();
        return currentStat;
    }

    public void ClearGear()
    {
        currentGear = null;
        currentStat = SlotStat.Incorrect;
        Debug.Log($"Slot {name}: Gear cleared, stat={currentStat}");
        gearPuzzle.UpdateGearStates();
    }
}