using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class TeaCheck : IInteractable
{
    public Item item;
    public GameObject token;
    public bool isToken;
    public bool isDrinked;
    public bool inSlot;
    public Transform firstPosition;
    private TeaLever tL;
    void Start()
    {
        EvntManager.StartListening("SetToken", SetToken);
        EvntManager.StartListening("SetTea", SetTea);
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
