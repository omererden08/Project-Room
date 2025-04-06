using UnityEngine;

public class PuzzleMagnetFinish : MonoBehaviour
{
    public PuzzleManager puzzleManager;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ball")
        {
            puzzleManager.PuzzleSolved();
            Destroy(gameObject);
        }
    }

}
