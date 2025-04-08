using UnityEngine;

public class End : MonoBehaviour
{

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Quit();
        }
    }

    void Quit()
    {
        Application.Quit();
        Debug.Log("Quit");
    }
}
