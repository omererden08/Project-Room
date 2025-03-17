using UnityEngine;
using UnityEngine.InputSystem;
public class ReadCheck : MonoBehaviour
{
    private FirstPersonController controller;
    void Start()
    {
        controller = FindAnyObjectByType<FirstPersonController>();
    }
    void OnRead()
    {
        Debug.Log("OnRead triggered");
        if (controller.CurrentInteractable is Paper paper)
        {
            Debug.Log(paper.content);
        }
        else
        {
            Debug.Log("IInteractable is not a paper");
        }
    }
}
