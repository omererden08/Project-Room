using UnityEngine;

public class NextScene : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            FadeManager.Instance.FadeToNextScene();
        }
    }
}
