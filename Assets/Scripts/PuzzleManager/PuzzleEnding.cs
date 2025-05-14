using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
public class PuzzleEnding : MonoBehaviour
{
    public PuzzleManager[] puzzleManagers;
    void Awake() => puzzleManagers = FindObjectsOfType<PuzzleManager>();
    void Start()
    {
        Debug.Log("Puzzles found: " + puzzleManagers.Length);
    }

    public bool EndCheck()
    {
        if (puzzleManagers.All(pm => pm.solved))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
