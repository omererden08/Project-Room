using System.Diagnostics.Contracts;
using UnityEngine;

public class TeaLight : MonoBehaviour
{
    public Animator animator;

    public void Start()
    {
        animator = GetComponent<Animator>();
        EvntManager.StartListening("SetOnSlotLight", SetOnSlotLight);
        EvntManager.StartListening("SetOffSlotLight", SetOffSlotLight);
        EvntManager.StartListening("SetOnAllLight", SetOnAllLight);
        EvntManager.StartListening("SetOffAllLight", SetOffAllLight);
    }

    public void SetOnSlotLight()
    {
        //animator.SetTrigger("OnSlotLight");
        animator.SetBool("OnSlot", true);
    }
    public void SetOffSlotLight()
    {
        //animator.SetTrigger("OffSlotLight");
        animator.SetBool("OnSlot", false);

    }
    public void SetOnAllLight()
    {
        //animator.SetTrigger("OnAllLight");
        animator.SetBool("OnAll", true);
    }
    public void SetOffAllLight()
    {
        //animator.SetTrigger("OffAllLight");
        animator.SetBool("OnAll", false);
    }
}
