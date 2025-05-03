using System.Xml.Serialization;
using UnityEngine;

public class DoorAnimation : MonoBehaviour
{
    public Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        EvntManager.StartListening<int>("PlayDoorAnimation",PlayAnimationSequence);
    }

    void PlayAnimationSequence(int animationID)
    {
        animator.SetTrigger(animationID.ToString());
    }
}
