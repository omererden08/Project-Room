using UnityEngine;
using System.Collections.Generic;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }
    public List<Item> items = new List<Item>();
    public InventorySlot[] slots;
    public bool isPuzzleMode { get; private set; }
    private const int MAX_SLOTS = 8;

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
        if (item == null || string.IsNullOrEmpty(item.itemName))
        {
            Debug.LogWarning("AddItem: Geçersiz item veya itemName");
            return false;
        }

        Item existingItem = items.Find(i => i.itemName == item.itemName);
        if (existingItem != null)
        {
            existingItem.quantity += item.quantity;
            if (item.sceneObjects != null && item.sceneObjects.Count > 0)
            {
                foreach (var obj in item.sceneObjects)
                {
                    if (obj != null && !existingItem.sceneObjects.Contains(obj))
                    {
                        if (obj.activeSelf) obj.SetActive(false); // Yalnızca aktifse gizle
                        existingItem.sceneObjects.Add(obj);
                    }
                }
            }
            UpdateSlots();
            return true;
        }

        if (items.Count >= MAX_SLOTS)
        {
            Debug.Log("Envanter dolu! Yeni item eklenemedi: " + item.itemName);
            return false;
        }
        //BURADA BİR ŞEYLER VAR
        items.Add(item);
        UpdateSlots();
        return true;
    }

    public void RemoveItem(string itemName, int quantity)
    {
        if (string.IsNullOrEmpty(itemName))
        {
            Debug.LogWarning("RemoveItem: Geçersiz itemName");
            return;
        }

        Item existingItem = items.Find(i => i.itemName == itemName);
        if (existingItem != null)
        {
            existingItem.quantity -= quantity;
            if (existingItem.quantity <= 0)
            {
                existingItem.sceneObjects.Clear();
                items.Remove(existingItem);
            }
            else
            {
                for (int i = 0; i < quantity && existingItem.sceneObjects.Count > 0; i++)
                {
                    existingItem.sceneObjects.RemoveAt(existingItem.sceneObjects.Count - 1);
                }
                // Null nesneleri temizle
                existingItem.sceneObjects.RemoveAll(obj => obj == null);
            }
            Debug.Log($"RemoveItem: Item = {itemName}, Quantity = {existingItem.quantity}, SceneObjects Count = {existingItem.sceneObjects.Count}");
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
                Debug.Log($"UpdateSlots: Slot {i}: {items[i].itemName}, Quantity: {items[i].quantity}, SceneObjects Count: {items[i].sceneObjects.Count}");
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }
}