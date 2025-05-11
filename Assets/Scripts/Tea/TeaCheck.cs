using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class TeaCheck : IInteractable
{
    public GameObject token;
    public bool isToken;
    public bool isDrinked;
    public bool inSlot;
    public Transform firstPosition;
    private TeaLever tL;
    private TeaSlot teaSlot;
    void Start()
    {
        Debug.Log("here>>>>>>>>>>>>>>>>>>>>>>>>>>>");
        EvntManager.StartListening("SetToken", SetToken);
        EvntManager.StartListening("SetTea", SetTea);
        teaSlot = FindAnyObjectByType<TeaSlot>();
        outline = GetComponent<Outline3D>();
        outline.enabled = false;
        tL = FindAnyObjectByType<TeaLever>();
        SetTea();
        firstPosition = transform;
        isDrinked = false;
        token.SetActive(false);
    }
    void SetToken()
    {
        transform.position = firstPosition.position;
        
        isToken = true;
        token.SetActive(true);

    }
    void SetTea()
    {
        transform.position = firstPosition.position;
        isToken = false;
        tL.canLeverPull = true;    
        token.SetActive(false);
    }

    public override void PickUp()
    {
        if (!isToken) 
        {
            if(!isDrinked)
            {
                EvntManager.TriggerEvent("DrinkTea");
                isDrinked = true;
            }
        }
        else
        {
                SetToken();
        }
        base.PickUp();
    }
}
