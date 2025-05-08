using System.Linq;
using UnityEngine;

public class GetActivePuzzleManager : MonoBehaviour
{
    public PuzzleManager[] puzzleManagers;
    void Awake() => puzzleManagers = FindObjectsOfType<PuzzleManager>();
    public PuzzleManager GetPuzzleManager() { return puzzleManagers.FirstOrDefault(pm => pm.inPuzzleMode); }
    public Transform PuzzleObjectRoot()
    {
        return GetPuzzleManager().puzzleRootTransform;
    }
}
