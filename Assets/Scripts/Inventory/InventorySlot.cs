using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Unity.Mathematics;

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
    }

    public void ClearSlot()
    {
        item = null;
        itemIcon.sprite = null;
        itemIcon.enabled = false;
        quantityText.text = "";
        NotVisibleSlot();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item == null || item.sceneObjects.Count == 0)
        {
            Debug.Log("OnBeginDrag: Item null veya sceneObjects boş");
            return;
        }

        originalPosition = transform.position;
        dragStartPosition = eventData.position;
        activeObject = item.sceneObjects.Find(obj => obj != null);

        if (activeObject != null)
        {
            activeObject.SetActive(true);
            Debug.Log($"OnBeginDrag: Active Object {activeObject.name} aktif edildi");
        }
        else
        {
            Debug.Log("OnBeginDrag: Geçerli activeObject bulunamadı");
            return;
        }

        Cursor.visible = true;
        puzzleTransform = gAPM.GetPuzzleManager()?.transform;

        if (puzzleTransform != null)
        {
            Vector3 worldPos = GetMouseWorldPosition(eventData);
            PuzzleDirection direction = gAPM.GetPuzzleManager().direction;
            AdjustObjectBasedOnPuzzleDirection(worldPos, direction);
            Debug.Log($"OnBeginDrag: Begin drag with direction = {direction}");
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item == null || activeObject == null || puzzleTransform == null)
        {
            Debug.Log("OnDrag: Item, activeObject veya puzzleTransform null");
            return;
        }

        Vector3 worldPos = GetMouseWorldPosition(eventData);
        PuzzleDirection direction = gAPM.GetPuzzleManager().direction;
        AdjustObjectBasedOnPuzzleDirection(worldPos, direction);
        Debug.Log($"OnDrag: Item position = {activeObject.transform.position}");
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
            Debug.Log("OnEndDrag: Item veya activeObject null");
            return;
        }

        Debug.Log($"OnEndDrag: Active Object = {activeObject.name}, SceneObjects Count = {item.sceneObjects.Count}");

        if (puzzleTransform == null)
        {
            activeObject.SetActive(false);
            Debug.Log("OnEndDrag: PuzzleTransform null, nesne gizlendi");
            return;
        }

        float distanceToPuzzle = Vector3.Distance(activeObject.transform.position, puzzleTransform.position);
        Debug.Log($"OnEndDrag: Distance to puzzle = {distanceToPuzzle}");

        if (distanceToPuzzle < 1)
        {
            InventorySystem.Instance.RemoveItem(item.itemName, 1);
            Debug.Log($"AAAAAAAAAAAAAAAAAAA = {item.itemName}");
        }
        else
        {
            activeObject.SetActive(false);
            Debug.Log($"OnEndDrag: Active Object {activeObject.name} gizlendi");
        }

        transform.position = originalPosition;
        activeObject = null; // activeObject'i sıfırla
    }

    public void OnDrop(PointerEventData eventData)
    {
        // Slotlar arası değiştirme (isteğe bağlı)
    }

    private void VisibleSlot()
    {
        itemIcon.color = new Color(255, 255, 255, 255);
    }

    private void NotVisibleSlot()
    {
        itemIcon.color = new Color(255, 255, 255, 0);
    }
}