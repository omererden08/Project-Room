using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int maxSlots = 7;
    [SerializeField] private Image inventoryUI;
    [SerializeField] private bool isInventoryOpen;
    [SerializeField] private Image[] slots = new Image[7];
    private Image slotImage;

    public List<InventoryItem> inventory = new List<InventoryItem>();

    void Start()
    {
        inventoryUI = GameObject.Find("InventoryUI").GetComponent<Image>();
        inventoryUI.gameObject.SetActive(false);

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = inventoryUI.transform.GetChild(i).GetComponent<Image>();
            slotImage = slots[i].GetComponent<Image>();
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            OnInventoy();
        }

    }
public bool AddItem(Item item, int quantity = 1)
{
    InventoryItem existingItem = inventory.Find(i => i.item == item);

    if (existingItem != null && item.isStackable)
    {
        existingItem.quantity += quantity;

        // Stack edilen item'� listenin ba��na al
        inventory.Remove(existingItem);
        inventory.Insert(0, existingItem);
        UpdateUI();

        Debug.Log($"Stacked and moved {item.itemName} to front. Quantity: {existingItem.quantity}");
        return true;
    }
    else
    {
        if (inventory.Count >= maxSlots)
        {
            Debug.Log("Inventory is full, cannot add item.");
            return false;
        }

        inventory.Insert(0, new InventoryItem(item, quantity));
        Debug.Log($"Added {item.itemName} x{quantity} to front");
        UpdateUI();
        return true;
    }
}

    public void RemoveItem(Item item, int quantity = 1)
    {
        InventoryItem existingItem = inventory.Find(i => i.item == item);
        if (existingItem != null)
        {
            existingItem.quantity -= quantity;
            if (existingItem.quantity <= 0)
            {
                inventory.Remove(existingItem);
                UpdateUI();
            }

            Debug.Log($"Removed {item.itemName}, remaining: {existingItem.quantity}");
        }

    }

    private void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < inventory.Count)
            {
                slots[i].sprite = inventory[i].item.itemSprite;
                slots[i].color = Color.white;
            }
            else
            {
                slots[i].sprite = slotImage.sprite; 
                //slots[i].color = new Color(1, 1, 1, 0); // bo� slotu g�r�nmez yap
            }
        }
    }

    void OnInventoy()
    {
        isInventoryOpen = !isInventoryOpen;
        inventoryUI.gameObject.SetActive(isInventoryOpen);
    }

}
