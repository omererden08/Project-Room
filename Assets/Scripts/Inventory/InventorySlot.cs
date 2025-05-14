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
    private GameObject activeObject; // Sürüklenen nesneyi sakla
    private List<GameObject> activeObjects = new List<GameObject>();
    public int ct;
    private const float Offset = 0.05f;

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
        
        // Eğer öğe ekleniyorsa ve ilk kez sahneye konuyorsa active objects listesini temizle
        if (!activeObjects.Contains(newItem.sceneObjects[0]) && newItem.sceneObjects.Count > 0)
        {
            activeObjects.Clear();
            foreach (GameObject sceneObj in newItem.sceneObjects)
            {
                if (sceneObj != null)
                {
                    activeObjects.Add(sceneObj);
                    sceneObj.SetActive(false); // Başlangıçta deaktif
                }
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
        if (item == null || item.sceneObjects.Count == 0 || item.quantity <= 0)
        {
            return;
        }

        originalPosition = transform.position;
        dragStartPosition = eventData.position;
        
        // Sadece bir nesneyi aktifleştir, diğerlerini devre dışı bırak
        activeObject = null;
        foreach (GameObject obj in item.sceneObjects)
        {
            if (obj != null)
            {
                if (activeObject == null)
                {
                    activeObject = obj;
                    activeObject.SetActive(true);
                }
                else
                {
                    obj.SetActive(false);
                }
            }
        }

        if (activeObject == null)
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
                newPosition.x = puzzlePos.x;
                activeObject.transform.rotation = Quaternion.Euler(0, 90, 0); // Simplify rotation calculation
                newPosition.y += Offset;
                break;
            case PuzzleDirection.y:
                activeObject.transform.rotation = Quaternion.Euler(0, 0, 0); // Simplify rotation calculation
                newPosition.x += Offset;
                break;
        }

        float distanceToPuzzle = (newPosition - puzzlePos).magnitude; // Use magnitude property for distance calculation
        if (distanceToPuzzle > interactionDistance)
        {
            newPosition = puzzlePos + (newPosition - puzzlePos).normalized * interactionDistance;
        }

        activeObject.transform.position = newPosition;
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

        // Bu satır silinmiş çünkü tüm nesneleri devre dışı bırakıyordu
        // item.FalseAll();

        if (!gAPM.GetPuzzleManager().isAccepted(item))
        {
            GetBackItem();
            EvntManager.TriggerEvent("UpdateSlots");
            return;
        }

        if (puzzleTransform == null)
        {
            activeObject.SetActive(false);
            return;
        }

        float distanceToPuzzle = Vector3.Distance(activeObject.transform.position, puzzleTransform.position);
        if (distanceToPuzzle < 1)
        {
            GameObject copy = Instantiate(activeObject, activeObject.transform.position, activeObject.transform.rotation);
            //if(copy.GetComponent<Gear>() != null) copy.GetComponent<Gear>().();
            InventorySystem.Instance.RemoveItem(item.itemName, 1);
            copy.SetActive(true);
        }
        else
        {
            InventorySystem.Instance.RemoveItem(item.itemName, 1);
        }

        // Sadece aktif nesneyi gizleyin, diğerlerine dokunmayın
        activeObject.SetActive(false);
        activeObject = null;
        transform.position = originalPosition;
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