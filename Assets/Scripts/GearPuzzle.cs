using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GearPuzzle : MonoBehaviour
{
    public Slot[] slots; // Slotlar sıralı olmalı (1, 2, 3, 4, 5 gibi)

    void Start()
    {
        if (slots == null || slots.Length == 0)
        {
            Debug.LogError("Slots dizisi boş veya null! Lütfen Inspector'dan doldurun.");
        }
    }

    public void CheckPuzzleCompletion()
    {
        if (slots == null || slots.Length == 0)
        {
            Debug.Log("Slots dizisi boş veya null!");
            return;
        }

        bool allCorrect = true;
        for (int i = 0; i < slots.Length; i++)
        {
            if (!slots[i].isCorrect)
            {
                allCorrect = false;
                break;
            }
        }

        if (allCorrect)
        {
            Debug.Log("Puzzle Completed!");
        }
        else
        {
            Debug.Log("Puzzle not completed!");
        }
    }

    public bool CanGearSpin(int slotIndex)
    {
        if (slotIndex == 0) return true; // İlk çark her zaman dönebilir
        
        for (int i = 0; i < slotIndex; i++)
        {
            if (!slots[i].isCorrect)
            {
                return false;
            }
        }
        return true;
    }

    public void UpdateAllGears()
    {
        // Eski metod: FindObjectsOfType
        Gear[] allGears = UnityEngine.Object.FindObjectsOfType<Gear>();
        foreach (Gear gear in allGears)
        {
            if (gear.CurrentSlot != null) // Getter ile erişiyoruz
            {
                gear.CheckAndUpdateSpinning();
            }
        }
        CheckPuzzleCompletion();
    }
}