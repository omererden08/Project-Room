using UnityEngine;

public class Finish : MonoBehaviour
{
    [SerializeField] private GameObject ball;
    [SerializeField] private Vector3 targetScale = new Vector3(0.5f, 0.5f, 0.5f);
    [SerializeField] private float scaleSpeed;
    private bool isFinished = false;
    [SerializeField] PuzzleManager puzzleManager;
    void Start()
    {
        //puzzleManager = GetComponentInParent<PuzzleManager>();
    }
    void Update()
    {
        if (isFinished)
        {
            FinishPuzzle();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered");
        if (other.CompareTag("Ball"))
        {
            isFinished = true;
        }
    }

    void FinishPuzzle()
    {
        
            ball.SetActive(false);
            print("Puzzle Finished");
            puzzleManager.PuzzleSolved();
            //puzzleManager solved olacak 
        
    }



}
