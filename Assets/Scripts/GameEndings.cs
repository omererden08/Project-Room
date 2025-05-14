using UnityEngine;

public class GameEndings : MonoBehaviour
{
    public PuzzleEnding puzzleEnding;
    public AudioClip bombSound;

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
            AudioManager.Instance.audioSource.volume = 0.2f;
            AudioManager.Instance.audioSource.PlayOneShot(bombSound);
            FadeManager.Instance.BlackScene("MainMenu");
        }
    }

    
}
