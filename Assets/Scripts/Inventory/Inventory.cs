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
    [SerializeField] private float dragDepthMultiplier = 0.5f;
    [SerializeField] private float mouseDragSmoothness = 5f;



    public List<GameObject> inv;

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
            Debug.LogError("Required components not assigned!");
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

        if (Input.GetMouseButtonDown(0) && !isDragging && isInventoryOpen)
        {
            DraggingItem();
        }

        if (isDragging && draggedObject != null)
        {
            UpdateDraggedObjectPosition();
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            DropItem();
        }
    }

    public bool AddItem(Item item, int quantity = 1, GameObject originalObject = null)
    {
        if (item == null)
        {
            Debug.LogWarning("Attempted to add null item to inventory!");
            return false;
        }
        //envanter kontrolu saglanacak
        inv.Add(originalObject);
        InventoryItem existingItem = inventory.Find(i => i.item == item && i.originalObject == originalObject);

        if (existingItem != null && item.isStackable)
        {
            existingItem.quantity += quantity;
            inventory.Remove(existingItem);
            inventory.Insert(0, existingItem);
            UpdateSlot(0);
            return true;
        }
        else
        {
            if (inventory.Count >= maxSlots)
            {
                Debug.Log("Inventory is full!");
                return false;
            }

            inventory.Insert(0, new InventoryItem(item, quantity, originalObject));
            UpdateUI();
            return true;
        }
    }

    public void RemoveItem(Item item, int quantity = 1, GameObject originalObject = null)
    {
        if (item == null)
        {
            Debug.LogWarning("Attempted to remove null item from inventory!");
            return;
        }

        InventoryItem existingItem = inventory.Find(i => i.item == item && i.originalObject == originalObject);
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

        UpdateUI();
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
            if (slot == null || slot.inventoryItem == null || slot.inventoryItem.item == null || slot.inventoryItem.originalObject == null)
            {
                continue;
            }

            PuzzleManager activePuzzle = GetActivePuzzleManager();
            if (activePuzzle == null)
            {
                Debug.LogWarning("No active PuzzleManager found!");
                return;
            }

            // Use the original object instead of instantiating
            draggedObject = slot.inventoryItem.originalObject;
            draggedObject.SetActive(true); // Reactivate the object

            ObjectsCreatedFromInventory.Add(draggedObject);
            Debug.Log($"Added {slot.inventoryItem.item.itemName} to ObjectsCreatedFromInventory. Total objects: {ObjectsCreatedFromInventory.Count}");
            isDragging = true;

            // Remove the item from inventory
            RemoveItem(slot.inventoryItem.item, 1, slot.inventoryItem.originalObject);

            // Disable physics to prevent unwanted movement while dragging
            var rb = draggedObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }

            // Ensure all renderers are enabled
            var renderers = draggedObject.GetComponentsInChildren<Renderer>(true);
            foreach (var renderer in renderers)
            {
                renderer.enabled = true;
            }

            // Ensure all colliders are enabled but set to trigger during drag
            var colliders = draggedObject.GetComponentsInChildren<Collider>(true);
            foreach (var collider in colliders)
            {
                collider.enabled = true;
                collider.isTrigger = true;
            }

            Debug.Log($"Item reactivated for dragging: {slot.inventoryItem.item.itemName}");
            break;
        }
    }

    void UpdateDraggedObjectPosition()
    {
        if (draggedObject == null) return;

        PuzzleManager activePuzzle = GetActivePuzzleManager();
        if (activePuzzle == null)
        {
            Debug.LogWarning("No active PuzzleManager found during drag!");
            DropItem();
            return;
        }

        float dragDepth = Vector3.Distance(cam.transform.position, activePuzzle.cameraFocusPoint.position) * dragDepthMultiplier;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Vector3 targetPosition = ray.origin + ray.direction * dragDepth;

        draggedObject.transform.position = Vector3.Lerp(
            draggedObject.transform.position,
            targetPosition,
            Time.deltaTime * mouseDragSmoothness
        );
    }

    void DropItem()
    {
        if (draggedObject != null)
        {
            var rb = draggedObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }

            var colliders = draggedObject.GetComponentsInChildren<Collider>(true);
            foreach (var collider in colliders)
            {
                collider.isTrigger = false;
            }

            Debug.Log($"Item dropped: {draggedObject.name}");
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