using UnityEngine;

public class TeaSlot : MonoBehaviour
{
    public GameObject tea;
    public Vector3 spawnPoint;

    public bool isFilled = true;

    private InventorySystem inventorySystem;

    void Start()
    {
        EvntManager.StartListening("FillSlot", FillSlot);
        isFilled = true;
        EvntManager.TriggerEvent("SetOnSlotLight");

        inventorySystem = FindAnyObjectByType<InventorySystem>();

    }


    void OnMouseDown()
    {
        if (!isFilled && inventorySystem.ChosenItem("TeaCup"))
        {
            FillSlot();
        }
    }
    void FillSlot()
    {
        EvntManager.TriggerEvent("MoveCupToStart");
        inventorySystem.RemoveItem("TeaCup", 1);
        isFilled = true;
        EvntManager.TriggerEvent("SetOffSlotLight");

        EvntManager.TriggerEvent("SetTea"); // SetTea olayını tetikle
        Debug.Log("TeaSlot dolduruldu. isFilled: " + isFilled);
    }
}
