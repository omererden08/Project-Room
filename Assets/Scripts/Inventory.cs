using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public int maxSlots = 7;
    public Image inventoryUI;
    private bool isInventoryOpen;
    public Image[] slots = new Image[7];

    public List<InventoryItem> inventory = new List<InventoryItem>();

    void Start()
    {
        inventoryUI = GameObject.Find("InventoryUI").GetComponent<Image>();
        inventoryUI.gameObject.SetActive(false);

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = inventoryUI.transform.GetChild(i).GetComponent<Image>();
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            isInventoryOpen = !isInventoryOpen;
            inventoryUI.gameObject.SetActive(isInventoryOpen);
        }

    }

    public void AddItem(Item item, int quantity = 1)
    {
        InventoryItem existingItem = inventory.Find(i => i.item == item);

        if (existingItem != null && item.isStackable)
        {
            existingItem.quantity += quantity;

            // Stack edilen item'ý listenin baþýna al
            inventory.Remove(existingItem);
            inventory.Insert(0, existingItem);
            UpdateUI();

            Debug.Log($"Stacked and moved {item.itemName} to front. Quantity: {existingItem.quantity}");
        }
        else
        {
            // Yeni item'ý baþa ekle
            inventory.Insert(0, new InventoryItem(item, quantity));
            Debug.Log($"Added {item.itemName} x{quantity} to front");

            // Slot kapasitesini aþarsa sondakini sil
            if (inventory.Count > maxSlots)
            {
                inventory.RemoveAt(inventory.Count - 1);
            }
        }

        UpdateUI();
    }

    public void RemoveItem(Item item, int quantity = 1)
    {
        InventoryItem existingItem = inventory.Find(i => i.item == item);
        if (existingItem != null)
        {
            existingItem.quantity -= quantity;
            if (existingItem.quantity <= 0)
                inventory.Remove(existingItem);

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
                slots[i].sprite = null;
                slots[i].color = new Color(1, 1, 1, 0); // boþ slotu görünmez yap
            }
        }
    }


}
