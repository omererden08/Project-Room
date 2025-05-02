using UnityEngine;
using UnityEngine.InputSystem;
public class ReadCheck : MonoBehaviour
{
    private InteractionSystem controller;
    void Start()
    {
        controller = FindAnyObjectByType<InteractionSystem>();
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
