using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using UnityEngine;

public class DoorAnimation : IInteractable
{
    public Animator animator;
    public InventorySystem Inventory;
    public bool isLocked = false;
    public int tokenTaken;
    private Collider doorCollider;
    void Start()
    {
        Inventory = FindAnyObjectByType<InventorySystem>();
        animator = GetComponent<Animator>();
        doorCollider = GetComponent<Collider>();
        tokenTaken = 0;
    }

    public void LockDoor()
    {
        animator.SetTrigger("Door_Lock");
        EvntManager.TriggerEvent("BombUpStart");
        isLocked = true;
    }

    public void ShakeStart()
    {
        EvntManager.TriggerEvent("CameraShakeLittle");
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
                animator.SetTrigger("Door_Unlock_" + tokenTaken);
                tokenTaken++;

            }


            switch (tokenTaken)
            {
                case 0:
                    Inventory.RemoveItem("Token", 1);
                    EvntManager.TriggerEvent("Door_Unlock_1");
                    tokenTaken++;

                    break;
                case 1:
                    Inventory.RemoveItem("Token", 1);
                    EvntManager.TriggerEvent("Door_Unlock_2");
                    tokenTaken++;


                    break;
                case 2:
                    Inventory.RemoveItem("Token", 1);
                    EvntManager.TriggerEvent("Door_Unlock_3");
                    tokenTaken++;

                    break;
                case 3:
                    Inventory.RemoveItem("Token", 1);
                    EvntManager.TriggerEvent("Door_Unlock_4");
                    tokenTaken++;

                    break;
                case 4:
                    animator.SetTrigger("Door_Unlock_All");
                    doorCollider.enabled = false;
                    GameEnding();
                    break;
            }
        }
        base.Interact();
    }

    public void GameEnding()
    {
        Debug.Log("game ending");
    }
}
