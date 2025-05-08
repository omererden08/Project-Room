using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Unity.Mathematics;
using System.Collections.Generic;
using Unity.VisualScripting;

public enum PuzzleDirection
{
    x,  // X eksenine bakan puzzle
    y   // Y eksenine bakan puzzle
}

public class InventorySlot : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler
{
    public Image itemIcon;
    public TextMeshProUGUI quantityText;
    private Item item;
    private Vector2 originalPosition;
    public GetActivePuzzleManager gAPM;
    [SerializeField] private float zCoordinate = 10f;
    [SerializeField] private float interactionDistance = 2f;
    private Camera mainCamera;
    private Vector2 dragStartPosition;
    private Transform puzzleTransform;
    public float offset;
    private GameObject activeObject; // Sürüklenen nesneyi sakla
    private List<GameObject> activeObjects = new List<GameObject>();
    public int ct;

    private void Awake()
    {
        quantityText = GetComponentInChildren<TextMeshProUGUI>();
        itemIcon = GetComponent<Image>();
        gAPM = FindAnyObjectByType<GetActivePuzzleManager>();
        mainCamera = Camera.main;
    }

    private void Start()
    {
        NotVisibleSlot();
    }

    public void SetItem(Item newItem)
    {
        if (newItem == null) return;

        VisibleSlot();
        item = newItem;
        itemIcon.sprite = item.icon;
        itemIcon.enabled = true;
        quantityText.text = item.quantity > 0 ? item.quantity.ToString() : "";
        // Add scene object only if not already in activeObjects
        if (newItem.sceneObjects.Count > 0)
        {
            GameObject sceneObj = newItem.sceneObjects[newItem.sceneObjects.Count - 1];
            if (sceneObj != null && !activeObjects.Contains(sceneObj))
            {
                activeObjects.Add(sceneObj);
            }
        }
        ct = newItem.sceneObjects.Count - 1;
    }

    public void ClearSlot()
    {
        item = null;
        itemIcon.sprite = null;
        itemIcon.enabled = false;
        quantityText.text = "";
        activeObjects.Clear();
        NotVisibleSlot();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item == null || item.sceneObjects.Count == 0)
        {
            return;
        }

        originalPosition = transform.position;
        dragStartPosition = eventData.position;
        activeObject = item.sceneObjects.Find(obj => obj != null);

        if (activeObject != null)
        {
            activeObject.SetActive(true);
            if (!activeObjects.Contains(activeObject))
            {
                activeObjects.Add(activeObject);
            }
        }
        else
        {
            return;
        }

        Cursor.visible = true;
        puzzleTransform = gAPM.GetPuzzleManager()?.transform;

        if (puzzleTransform != null)
        {
            Vector3 worldPos = GetMouseWorldPosition(eventData);
            PuzzleDirection direction = gAPM.GetPuzzleManager().direction;
            AdjustObjectBasedOnPuzzleDirection(worldPos, direction);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item == null || activeObject == null || puzzleTransform == null)
        {
            return;
        }

        Vector3 worldPos = GetMouseWorldPosition(eventData);
        PuzzleDirection direction = gAPM.GetPuzzleManager().direction;
        AdjustObjectBasedOnPuzzleDirection(worldPos, direction);
    }

    private void AdjustObjectBasedOnPuzzleDirection(Vector3 mouseWorldPos, PuzzleDirection direction)
    {
        if (activeObject == null || puzzleTransform == null) return;

        Vector3 puzzlePos = puzzleTransform.position;
        Vector3 newPosition = mouseWorldPos;

        switch (direction)
        {
            case PuzzleDirection.x:
                newPosition.z = puzzlePos.z;
                activeObject.transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
                newPosition.y += offset;
                break;
            case PuzzleDirection.y:
                newPosition.x = puzzlePos.x;
                activeObject.transform.rotation = Quaternion.LookRotation(Vector3.right, Vector3.up);
                newPosition.x += offset;
                break;
        }

        float distanceToPuzzle = Vector3.Distance(newPosition, puzzlePos);
        if (distanceToPuzzle > interactionDistance)
        {
            newPosition = puzzlePos + (newPosition - puzzlePos).normalized * interactionDistance;
        }

        if (ct >= 0 && activeObjects.Count > ct)
        {
            activeObjects[ct].transform.position = newPosition;
        }
    }

    private Vector3 GetMouseWorldPosition(PointerEventData eventData)
    {
        Vector3 mousePos = eventData.position;
        if (puzzleTransform != null)
        {
            Vector3 puzzleScreenPos = mainCamera.WorldToScreenPoint(puzzleTransform.position);
            mousePos.z = puzzleScreenPos.z;
        }
        else
        {
            mousePos.z = zCoordinate;
        }

        return mainCamera.ScreenToWorldPoint(mousePos);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (item == null || activeObject == null)
        {
            return;
        }

        item.FalseAll();
        if (!gAPM.GetPuzzleManager().isAccepted(item))
        {
            GetBackItem();
            if (ct >= 0 && item.sceneObjects.Count > ct)
            {
                item.sceneObjects[ct].SetActive(false);
            }
            return;
        }

        if (puzzleTransform == null)
        {
            if (activeObjects.Contains(activeObject))
            {
                activeObjects.Remove(activeObject);
            }
            return;
        }

        float distanceToPuzzle = Vector3.Distance(activeObject.transform.position, puzzleTransform.position);
        if (distanceToPuzzle < 1)
        {
            GameObject copy = Instantiate(activeObject, activeObject.transform.position, activeObject.transform.rotation);
            copy.SetActive(true);
            InventorySystem.Instance.RemoveItem(item.itemName, 1);
        }
        else
        {
            InventorySystem.Instance.RemoveItem(item.itemName, 1);
        }

        transform.position = originalPosition;
        activeObject = null;
    }

    public void GetBackItem()
    {
        if (activeObject != null)
        {
            activeObject.SetActive(false);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        // Slotlar arası değiştirme (isteğe bağlı)
    }

    private void VisibleSlot()
    {
        itemIcon.color = new Color(1f, 1f, 1f, 1f);
    }

    private void NotVisibleSlot()
    {
        itemIcon.color = new Color(1f, 1f, 1f, 0f);
    }
}