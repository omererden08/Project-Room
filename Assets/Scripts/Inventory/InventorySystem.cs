using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

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
        slots = slots.OrderBy(slot => slot.name).ToArray();
        if (slots.Length != MAX_SLOTS)
        {
            Debug.LogWarning($"Envanterde tam {MAX_SLOTS} slot olmalı! Şu an {slots.Length} slot var.");
        }
    }
    void Start()
    {
        EvntManager.StartListening("updateSlots", UpdateSlots);
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
        EvntManager.TriggerEvent("OpenInventory");

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

            // Check if the item quantity is less than or equal to 0
            if (existingItem.quantity < 0)
            {

                // Clear scene objects and remove the item from the list
                if (existingItem.sceneObjects != null)
                {
                    existingItem.sceneObjects.Clear();
                }
                items.Remove(existingItem);

                // Reset selected index if necessary
                if (selectedIndex >= items.Count && items.Count > 0)
                {
                    selectedIndex = items.Count - 1;
                }
                else if (items.Count == 0)
                {
                    selectedIndex = 0;
                }
            }
            else
            {
                // Remove scene objects if quantity is greater than 0
                if (existingItem.sceneObjects != null && existingItem.sceneObjects.Count > 0)
                {
                    for (int i = 0; i < quantity && i < existingItem.sceneObjects.Count; i++)
                    {
                        existingItem.sceneObjects.RemoveAt(existingItem.sceneObjects.Count - 1);
                    }
                    existingItem.sceneObjects.RemoveAll(obj => obj == null);
                }
            }

            // Log the removal
            Debug.Log($"RemoveItem: Item = {itemName}, Quantity = {existingItem.quantity}, SceneObjects Count = {(existingItem.sceneObjects != null ? existingItem.sceneObjects.Count : 0)}");

            // Update slots only if necessary
            if (existingItem.quantity <= 0 || existingItem.sceneObjects.Count == 0)
            {
                UpdateSlots();
            }
        }
    }

    void Update()
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
        UpdateSlotItems();
        HighlightSelectedSlot();
        ClearEmptySlots();
    }

    private void UpdateSlotItems()
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

 private void HighlightSelectedSlot()
{
    for (int i = 0; i < slots.Length; i++)
    {
        if (i == selectedIndex)
        {
            slots[i].GetComponent<Image>().sprite = items[i].icon; // Yellow highlight
        }
        else if (i < items.Count)
        {
            slots[i].GetComponent<Image>().sprite = items[i].outlinedIcon;
        }
        else
        {
            slots[i].GetComponent<Image>().sprite = null; // Set sprite to null for empty slots
        }
    }
}

    private void ClearEmptySlots()
    {
        List<Item> itemsToRemove = new List<Item>();
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].sceneObjects.Count == 0)
            {
                itemsToRemove.Add(items[i]);
            }
        }
        foreach (var item in itemsToRemove)
        {
            items.Remove(item);
        }
    }

    void SelectItem()
    {
        if (items == null || items.Count == 0) return; // Early return

        float scrollDelta = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scrollDelta) > 0 && Time.time - lastScrollTime > SCROLL_DEBOUNCE_TIME)
        {
            int scrollDirection = (int)Mathf.Sign(scrollDelta);
            Debug.Log("Scroll Direction: " + scrollDirection);

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