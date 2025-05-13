using UnityEngine;

public class GameEndings : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        FadeManager.Instance.BlackScene("MainMenu");
    }
}
