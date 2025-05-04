using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
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

    [SerializeField] public GraphicRaycaster raycaster;
    [SerializeField] public EventSystem eventSystem;

    [SerializeField] private List<InventoryItem> inventory = new List<InventoryItem>();
    [SerializeField] private List<GameObject> ObjectsCreatedFromInventory = new List<GameObject>();

    // Dragging variables
    private GameObject draggedObject;
    private bool isDragging;
    [SerializeField] private float dragDepthMultiplier = 0.5f; // Multiplier for dynamic drag depth

    void Start()
    {
        cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("Main Camera not found!");
            enabled = false;
            return;
        }

        if (inventoryUI == null || rectTransform == null || slots == null || slots.Length == 0 || 
            emptySlotSprite == null || raycaster == null || eventSystem == null)
        {
            Debug.LogError("Required components (InventoryUI, RectTransform, Slots, EmptySlotSprite, Raycaster, or EventSystem) not assigned!");
            enabled = false;
            return;
        }

        EvntManager.StartListening("OpenInventory", OpentoPuzzle);
        EvntManager.StartListening("CloseInventory", ClosetoPuzzle);

        initialPos = rectTransform.localPosition;
        targetPos = new Vector3(initialPos.x + 200f, initialPos.y, initialPos.z);

        if (slots.Length != maxSlots)
        {
            Debug.LogWarning($"Slot count ({slots.Length}) does not match maxSlots ({maxSlots}). Adjusting.");
            System.Array.Resize(ref slots, maxSlots);
        }

        UpdateUI();
    }

    void OnDestroy()
    {
        EvntManager.StopListening("OpenInventory", OpentoPuzzle);
        EvntManager.StopListening("CloseInventory", ClosetoPuzzle);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }

        if (Input.GetMouseButtonDown(0) && !isDragging)
        {
            DraggingItem();
        }

        if (isDragging && draggedObject != null)
        {
            UpdateDraggedObjectPosition();
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            StopDragging();
        }
    }

    public bool AddItem(Item item, int quantity = 1)
    {
        if (item == null)
        {
            Debug.LogWarning("Attempted to add null item to inventory!");
            return false;
        }

        InventoryItem existingItem = inventory.Find(i => i.item == item);

        if (existingItem != null && item.isStackable)
        {
            existingItem.quantity += quantity;
            inventory.Remove(existingItem);
            inventory.Insert(0, existingItem); // Stack edilen item'ı başa al
            UpdateSlot(0); // Update only the affected slot
            return true;
        }
        else
        {
            if (inventory.Count >= maxSlots)
            {
                Debug.Log("Inventory is full!");
                return false;
            }

            inventory.Insert(0, new InventoryItem(item, quantity));
            UpdateUI(); // Full UI update for new items
            return true;
        }
    }

    public void RemoveItem(Item item, int quantity = 1)
    {
        if (item == null)
        {
            Debug.LogWarning("Attempted to remove null item from inventory!");
            return;
        }

        InventoryItem existingItem = inventory.Find(i => i.item == item);
        if (existingItem == null)
        {
            Debug.LogWarning($"Item {item.itemName} not found in inventory!");
            return;
        }

        Debug.Log($"Removing {quantity} of {item.itemName} from inventory.");
        existingItem.quantity -= quantity;

        if (existingItem.quantity <= 0)
        {
            Debug.Log($"Item {item.itemName} quantity reached zero, removing from inventory.");
            inventory.Remove(existingItem);
        }

        UpdateUI(); // Full UI update to ensure all slots reflect the current inventory state
    }

    public Item GetItemAtSlot(int index)
    {
        if (index >= 0 && index < inventory.Count)
        {
            return inventory[index].item;
        }
        return null;
    }

    public List<GameObject> GetCreatedObjects()
    {
        return ObjectsCreatedFromInventory;
    }

    public void UpdateUI()
    {
        Debug.Log($"Inventory contents: {string.Join(", ", inventory.Select(i => $"{i.item.itemName} (x{i.quantity})"))}");
        for (int i = 0; i < slots.Length; i++)
        {
            UpdateSlot(i);
        }
    }

    private void UpdateSlot(int index)
    {
        if (index < 0 || index >= slots.Length) return;

        var slotUI = slots[index].GetComponent<InventorySlotUI>();
        if (slotUI == null)
        {
            slotUI = slots[index].gameObject.AddComponent<InventorySlotUI>();
        }

        if (index < inventory.Count)
        {
            slotUI.SetItem(inventory[index], emptySlotSprite);
        }
        else
        {
            slotUI.SetItem(null, emptySlotSprite);
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
        if (isMoving || isInventoryOpen) return;
        isInventoryOpen = true;
        StartCoroutine(MoveInventory(isInventoryOpen));
    }

    void ClosetoPuzzle()
    {
        if (isMoving || !isInventoryOpen) return;
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

        if (results.Count == 0)
        {
            Debug.Log("No valid inventory slot clicked.");
            return;
        }

        foreach (RaycastResult result in results)
        {
            var slot = result.gameObject.GetComponent<InventorySlotUI>();
            if (slot == null || slot.inventoryItem == null || slot.inventoryItem.item == null || slot.inventoryItem.item.itemPrefab == null)
            {
                continue;
            }

            PuzzleManager activePuzzle = GetActivePuzzleManager();
            if (activePuzzle == null)
            {
                Debug.LogWarning("No active PuzzleManager found!");
                return;
            }

            // Calculate dynamic drag depth
            float dragDepth = Vector3.Distance(cam.transform.position, activePuzzle.cameraFocusPoint.position) * dragDepthMultiplier;
            Vector3 spawnPos = activePuzzle.cameraFocusPoint.position + cam.transform.forward * dragDepth;

            // Instantiate the object, parented to the PuzzleManager
            draggedObject = Instantiate(slot.inventoryItem.item.itemPrefab, spawnPos, Quaternion.identity, activePuzzle.transform);
            ObjectsCreatedFromInventory.Add(draggedObject);
            Debug.Log($"Added {slot.inventoryItem.item.itemName} to ObjectsCreatedFromInventory. Total objects: {ObjectsCreatedFromInventory.Count}");
            isDragging = true;

            // Remove the item from inventory
            RemoveItem(slot.inventoryItem.item, 1);

            // Disable physics to prevent unwanted movement
            var rb = draggedObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }

            // Ensure the object is visible
            var meshRenderer = draggedObject.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.enabled = true;
            }

            Debug.Log($"Item instantiated for dragging: {slot.inventoryItem.item.itemName}");
            break; // Process only the first valid slot
        }
    }

    void UpdateDraggedObjectPosition()
    {
        if (draggedObject == null) return;

        PuzzleManager activePuzzle = GetActivePuzzleManager();
        if (activePuzzle == null)
        {
            Debug.LogWarning("No active PuzzleManager found during drag!");
            StopDragging();
            return;
        }

        // Calculate dynamic drag depth
        float dragDepth = Vector3.Distance(cam.transform.position, activePuzzle.cameraFocusPoint.position) * dragDepthMultiplier;

        // Convert mouse position to a world point
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Vector3.Distance(cam.transform.position, activePuzzle.cameraFocusPoint.position) + dragDepth;
        Vector3 worldPos = cam.ScreenToWorldPoint(mousePos);

        // Update the dragged object's position
        draggedObject.transform.position = worldPos;
    }

    void StopDragging()
    {
        if (draggedObject != null)
        {
            Destroy(draggedObject); // Explicitly clean up
            draggedObject = null;
        }
        isDragging = false;
    }

    private PuzzleManager GetActivePuzzleManager()
    {
        PuzzleManager[] puzzleManagers = FindObjectsOfType<PuzzleManager>();
        foreach (var pm in puzzleManagers)
        {
            if (pm.inPuzzleMode)
            {
                return pm;
            }
        }
        Debug.LogWarning("No active PuzzleManager found!");
        return null;
    }
}