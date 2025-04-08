using UnityEngine;
using UnityEngine.SceneManagement;
public class CheckEnd : MonoBehaviour
{
    public PuzzleManager[] managers;
    public void Check()
    {
        foreach (PuzzleManager manager in managers)
        {
            if (!manager.solved)
            {
                return;
            }
        }
        Debug.Log("All puzzles solved!");
        EndGame();
    }
    void EndGame()
    {

        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        SceneManager.LoadScene(nextSceneIndex);


    }
}
