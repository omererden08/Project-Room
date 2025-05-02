using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [Header("Inventory Config")]
    [SerializeField] private int maxSlots = 8;
    [SerializeField] private Image inventoryUI;
    [SerializeField] private Image[] slots;
    [SerializeField] private Sprite emptySlotSprite;


    [Header("UI Movement")]
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private float moveDuration = 0.3f;
    private Vector3 initialPos;
    private Vector3 targetPos;
    private bool isInventoryOpen;
    private bool isMoving;

    private List<InventoryItem> inventory = new List<InventoryItem>();

    void Start()
    {
        if (inventoryUI == null || rectTransform == null)
        {
            Debug.LogError("InventoryUI veya RectTransform atanmamış!");
            return;
        }

        initialPos = rectTransform.localPosition;
        targetPos = new Vector3(initialPos.x + 200f, initialPos.y, initialPos.z);

        if (slots.Length != maxSlots)
        {
            Debug.LogWarning("Slot sayısı maxSlots ile eşleşmiyor, güncelleyiniz.");
        }

        UpdateUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }
    }

    public bool AddItem(Item item, int quantity = 1)
    {
        InventoryItem existingItem = inventory.Find(i => i.item == item);

        if (existingItem != null && item.isStackable)
        {
            existingItem.quantity += quantity;
            inventory.Remove(existingItem);
            inventory.Insert(0, existingItem); // Stack edilen item'ı başa al
            UpdateUI();
            return true;
        }
        else
        {
            if (inventory.Count >= maxSlots)
            {
                return false; // Inventory is full
            }

            inventory.Insert(0, new InventoryItem(item, quantity));
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
            }

            UpdateUI();
        }
    }

    public void UpdateUI()
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
                slots[i].sprite = emptySlotSprite;
                slots[i].color = new Color(1, 1, 1, 0.3f);

            }
        }

    }

    void ToggleInventory()
    {
        if (isMoving) return;

        isInventoryOpen = !isInventoryOpen;
        StartCoroutine(MoveInventory(isInventoryOpen));
    }

    private IEnumerator MoveInventory(bool open)
    {
        isMoving = true;

        Vector3 start = rectTransform.localPosition;
        Vector3 end = open ? targetPos : initialPos;

        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            float t = elapsedTime / moveDuration;
            rectTransform.localPosition = Vector3.Lerp(start, end, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rectTransform.localPosition = end;
        isMoving = false;
    }
}
