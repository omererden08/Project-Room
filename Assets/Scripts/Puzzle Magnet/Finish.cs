using UnityEngine;

public class Finish : MonoBehaviour
{
    [SerializeField] private GameObject ball;
    [SerializeField] private Vector3 targetScale = new Vector3(0.5f, 0.5f, 0.5f);
    [SerializeField] private float scaleSpeed;
    private bool isFinished = false;
    public PuzzleManager puzzleManager;
    void Start()
    {
        puzzleManager = GetComponentInParent<PuzzleManager>();
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
        if (other.CompareTag("Ball"))
        {
            isFinished = true;
        }
    }

    void FinishPuzzle()
    {
        ball.transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleSpeed);
        if (transform.localScale.x <= 0.5f)
        {
            ball.SetActive(false);
            print("Puzzle Finished");
            puzzleManager.PuzzleSolved();
            //puzzleManager solved olacak 
        }
    }



}
