using UnityEngine;

public class TeaSlot : MonoBehaviour
{
    public GameObject tea;
    public Vector3 spawnPoint;

    public bool isFilled = true;
    private Outline3D o3D;
    private  InventorySystem inventorySystem;

    void Start()
    {
        isFilled = true;
        inventorySystem = FindAnyObjectByType<InventorySystem>();
        o3D = GetComponent<Outline3D>();
        o3D.enabled = false;
        CheckState();
    }
    
    public void CheckState()
    {
        if(isFilled)
        {
            o3D.enabled = false;    
        }
        else
        {
            o3D.enabled = true;
        }
    }
    void OnMouseDown()
    {
        Debug.Log((!isFilled && inventorySystem.ChosenItem("TeaCup")) + " " + !isFilled + " " + inventorySystem.ChosenItem("CupTea"));
        
        if(!isFilled && inventorySystem.ChosenItem("TeaCup"))
        {
            FillSlot();
        }
    }
    void FillSlot()
    {
        EvntManager.TriggerEvent("MoveCupToStart");
        inventorySystem.RemoveItem("TeaCup", 1);
        isFilled = true;
        CheckState();
        EvntManager.TriggerEvent("SetTea"); // SetTea olayını tetikle
        Debug.Log("TeaSlot dolduruldu. isFilled: " + isFilled);
    }
}
