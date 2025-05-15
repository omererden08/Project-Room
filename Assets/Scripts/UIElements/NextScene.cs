using UnityEngine;

public class NextScene : MonoBehaviour
{

    private void Start()
    {
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            FadeManager.Instance.FadeToNextScene();
        }
    }
}
