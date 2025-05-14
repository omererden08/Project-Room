using UnityEngine;

public class GameEndings : MonoBehaviour
{
    public PuzzleEnding puzzleEnding;
    void Start()
    {
        puzzleEnding = FindObjectOfType<PuzzleEnding>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(puzzleEnding.EndCheck() == true)
        {
            FadeManager.Instance.FadeWhite("MainMenu");
        }
        else
        {
            FadeManager.Instance.BlackScene("MainMenu");
        }
    }

    
}
