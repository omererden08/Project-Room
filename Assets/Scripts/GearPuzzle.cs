using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class GearPuzzle : MonoBehaviour
{
    public Slot[] slots;

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

        foreach (Slot slot in slots)
        {
            if (!slot.isCorrect)
            {
                Debug.Log("Puzzle not completed!");
                return;
            }
        }

        Debug.Log("Puzzle Completed!");
    }
}