using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
public class PuzzleEnding : MonoBehaviour
{
    public PuzzleManager[] puzzleManagers;
    public UnityAction AllPuzzlesSolvedAction;
    void Awake() => puzzleManagers = FindObjectsOfType<PuzzleManager>();

    public void EndCheck()
    {
        if(puzzleManagers.All(pm => pm.solved))
        {
            EndGame();
        }
    }

    public void EndGame()
    {
        AllPuzzlesSolvedAction.Invoke();
    }
}
