using UnityEngine;
using System.Collections.Generic;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }
    public List<Item> items = new List<Item>();
    public InventorySlot[] slots;
    public bool isPuzzleMode { get; private set; } // Puzzle modunu kontrol eder
    private const int MAX_SLOTS = 8; // Maksimum 8 slot

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        slots = FindObjectsOfType<InventorySlot>();
        if (slots.Length != MAX_SLOTS)
        {
            Debug.LogWarning($"Envanterde tam {MAX_SLOTS} slot olmalı! Şu an {slots.Length} slot var.");
        }
    }

    public bool AddItem(Item item)
    {
        // Aynı isimde item varsa stackle
        Item existingItem = items.Find(i => i.itemName == item.itemName);
        if (existingItem != null)
        {
            existingItem.quantity += item.quantity;
            if (existingItem.sceneObject != null)
                existingItem.sceneObject.SetActive(false); // Envantere eklenince nesneyi gizle
            UpdateSlots();
            return true;
        }

        // Yeni item için boş slot kontrolü
        if (items.Count >= MAX_SLOTS)
        {
            Debug.Log("Envanter dolu! Yeni item eklenemedi: " + item.itemName);
            return false;
        }

        // Yeni item ekle
        if (item.sceneObject != null)
            item.sceneObject.SetActive(false); // Yeni nesneyi gizle
        items.Add(item);
        UpdateSlots();
        return true;
    }

    public void RemoveItem(string itemName, int quantity)
    {
        Item existingItem = items.Find(i => i.itemName == itemName);
        if (existingItem != null)
        {
            existingItem.quantity -= quantity;
            if (existingItem.quantity <= 0)
            {
                items.Remove(existingItem);
            }
            UpdateSlots();
        }
    }

    public void SetPuzzleMode(bool active)
    {
        isPuzzleMode = active;
    }

    private void UpdateSlots()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < items.Count)
            {
                slots[i].SetItem(items[i]);
                Debug.Log($"Slot {i}: {items[i].itemName}, Miktar: {items[i].quantity}");
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }
}