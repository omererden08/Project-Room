using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


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

    private Camera cam;

    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;

    [SerializeField] private List<InventoryItem> inventory = new List<InventoryItem>();

    void Start()
    {
        cam = Camera.main;
        EvntManager.StartListening("OpenInventory", OpentoPuzzle);
        EvntManager.StartListening("CloseInventory", ClosetoPuzzle);

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

        if (Input.GetMouseButtonDown(0))
        {
            DraggingItem();
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

    public Item GetItemAtSlot(int index)
    {
        if (index >= 0 && index < inventory.Count)
        {
            return inventory[index].item;
        }
        return null;
    }

    public void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            var slotUI = slots[i].GetComponent<InventorySlotUI>();
            if (slotUI == null)
                slotUI = slots[i].gameObject.AddComponent<InventorySlotUI>();
                

            if (i < inventory.Count)
            {
                slotUI.SetItem(inventory[i], emptySlotSprite);
            }
            else
            {
                slotUI.SetItem(null, emptySlotSprite);
            }
        }
    }

    void ToggleInventory()
    {
        if (isMoving) return;

        isInventoryOpen = !isInventoryOpen;
        StartCoroutine(MoveInventory(isInventoryOpen));
    }

    void OpentoPuzzle()
    {
        if (isMoving) return;
        if (isInventoryOpen) return;
        isInventoryOpen = true;
        StartCoroutine(MoveInventory(isInventoryOpen));
    }

    void ClosetoPuzzle()
    {
        if (isMoving) return;
        if (!isInventoryOpen) return;
        isInventoryOpen = false;
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

    void DraggingItem()
    {
        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);

        foreach (RaycastResult result in results)
        {
            var slot = result.gameObject.GetComponent<InventorySlotUI>();

            if (slot == null || slot.inventoryItem == null || slot.inventoryItem.item == null || slot.inventoryItem.item.itemPrefab == null)
                continue;

            // Kamera pozisyonundan 5 birim önde instantiate etmek için
            Vector3 spawnPos = cam.transform.position + cam.transform.forward * 5f;

            // Instantiate edilen objeyi al
            GameObject spawnedObj = Instantiate(slot.inventoryItem.item.itemPrefab, spawnPos, Quaternion.identity);

            // Eğer MeshRenderer varsa, pozisyona göre görünürlüğünü kontrol et
            var meshRenderer = spawnedObj.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.enabled = false;

                if (spawnedObj.transform.position.x < 3f)
                {
                    meshRenderer.enabled = true;
                }
            }

            Debug.Log("Item instantiated from drag: " + slot.inventoryItem.item.itemName);
            break; // Birden fazla raycast sonucu varsa sadece ilkine tepki ver
        }
    }


}
