using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using UnityEngine;

public class DoorAnimation : IInteractable
{
    public Animator animator;
    public InventorySystem Inventory;
    public bool isLocked = false;
    public int tokenTaken;
    void Start()
    {
        Inventory = FindAnyObjectByType<InventorySystem>();
        animator = GetComponent<Animator>();
        tokenTaken = 0;
    }

    public void LockDoor()
    {
        animator.SetTrigger("Door_Lock");
        isLocked = true;
    }
    void PlayAnimationSequence(int animationID)
    {
        animator.SetTrigger(animationID.ToString());
    }
    override public void Interact()
    {
        if (!isLocked)
        {
            LockDoor();
            return;
        }

        if (Inventory.ChosenItem("Token"))
        {
            if (tokenTaken <= 4)
            {
                Inventory.RemoveItem("Token", 1);
                tokenTaken++;
                animator.SetTrigger("Door_Unlock_" + tokenTaken);
                return;
            }
            else
            {
                animator.SetTrigger("Door_Unlock_All");
                GameEnding();
                return;
            }

        }
        base.Interact();
    }

    public void GameEnding()
    {
        Debug.Log("game ending");
    }
}
