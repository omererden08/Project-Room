using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }
    public List<Item> items = new List<Item>();
    public InventorySlot[] slots;
    public bool isPuzzleMode { get; private set; }
    private const int MAX_SLOTS = 8;
    public int selectedIndex;
    private float lastScrollTime;
    private const float SCROLL_DEBOUNCE_TIME = 0.1f;

    public string selectedItemName;

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

        slots = FindObjectsByType<InventorySlot>(FindObjectsSortMode.None);
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

        // Check for stackable items
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
                        existingItem.sceneObjects.Add(obj);
                    }
                }
            }
            UpdateSlots();
            return true;
        }

        // Check if there’s an empty slot
        if (items.Count >= MAX_SLOTS)
        {
            Debug.Log("Envanter dolu! Yeni item eklenemedi: " + item.itemName);
            return false;
        }

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
                // Reset selectedIndex if the removed item was selected
                if (selectedIndex >= items.Count && items.Count > 0)
                {
                    selectedIndex = items.Count - 1;
                }
                else if (items.Count == 0)
                {
                    selectedIndex = 0;
                    CheckStationSelected(itemName);

                }
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
            Debug.Log($"RemoveItem: Item = {itemName}, Quantity = {(existingItem != null ? existingItem.quantity : 0)}, SceneObjects Count = {(existingItem != null ? existingItem.sceneObjects.Count : 0)}");

            UpdateSlots();
        }
    }

    void FixedUpdate()
    {
        SelectItem();
    }

    public void SetPuzzleMode(bool active)
    {
        isPuzzleMode = active;
    }
    public void CheckStationSelected(string nameOfItem)
    {
        if (selectedItemName == nameOfItem)
        {
            selectedItemName = "";
        }
    }

    private void UpdateSlots()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < items.Count)
            {
                slots[i].SetItem(items[i]);
                Debug.Log($"UpdateSlots: Slot {i}: {items[i].itemName}, Quantity: {items[i].quantity}, SceneObjects Count: {items[i].sceneObjects.Count}");
                // Highlight selected slot
                if (i == selectedIndex)
                {
                    slots[i].GetComponent<Image>().color = new Color(1f, 1f, 0.5f, 1f); // Yellow highlight
                }
                else
                {
                    slots[i].GetComponent<Image>().color = Color.white;
                }
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }

    void SelectItem()
    {
        if (items == null || items.Count == 0)
        {
            selectedIndex = 0;
            return;
        }

        float scrollDelta = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scrollDelta) > 0 && Time.time - lastScrollTime > SCROLL_DEBOUNCE_TIME)
        {
            int scrollDirection = (int)Mathf.Sign(scrollDelta);
            selectedIndex = Mathf.Clamp(selectedIndex - scrollDirection, 0, items.Count - 1); // Invert direction for natural scrolling
            lastScrollTime = Time.time;
            Debug.Log("Selected Item: " + (items.Count > 0 ? items[selectedIndex].itemName : "None"));
            selectedItemName = items[selectedIndex].itemName;
            UpdateSlots(); // Update UI to reflect selection
        }
    }

    public bool CheckItem(string nameOfItem)
    {
        if (items.Find(i => i.itemName == nameOfItem) != null)
        {
            return true;
        }
        else
            return false;
    }
    public bool ChosenItem(string nameOfItem)
    {
        if (selectedItemName == nameOfItem)
        {
            Debug.Log("selected item is: " + nameOfItem);
            return true;
        }
        else
            return false;
    }
}