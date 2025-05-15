using UnityEngine;

public class NextScene : MonoBehaviour
{
    private bool isClicked = false; 
    private void Start()
    {
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isClicked)
        {
            isClicked = true; // Týklama durumunu güncelle
            FadeManager.Instance.FadeToNextScene();
        }
    }
}
