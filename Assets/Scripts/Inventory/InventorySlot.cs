using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Unity.Mathematics;



public class InventorySlot : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler
{
    public Image itemIcon;
    public TextMeshProUGUI quantityText;
    private Item item;
    private Vector2 originalPosition;
    public GetActivePuzzleManager gAPM;
    
    // Kameranın uzaklık değeri
    [SerializeField] private float zCoordinate = 10f;
    
    // Etkileşim mesafesi
    [SerializeField] private float interactionDistance = 2f;
    
    // Puzzle yüzeyinden uzaklık offset değeri
    [SerializeField] private float puzzleOffset = 0.5f;
    
    private Camera mainCamera;
    
    // Sürüklemenin başladığı UI pozisyonu
    private Vector2 dragStartPosition;
    
    // Puzzle'ın transform'u
    private Transform puzzleTransform;

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
        VisibleSlot();
        item = newItem;
        itemIcon.sprite = item.icon;
        itemIcon.enabled = true;
        quantityText.text = item.quantity > 1 ? item.quantity.ToString() : "";
    }

    public void ClearSlot()
    {
        item = null;
        itemIcon.sprite = null;
        itemIcon.enabled = false;
        quantityText.text = "";
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item == null) return;
        
        originalPosition = transform.position;
        dragStartPosition = eventData.position;
        
        // Scene objesi aktifleştir
        item.sceneObject.SetActive(true);
        
        // Cursor'ı göster
        Cursor.visible = true;
        
        // Puzzle transform'unu kaydet
        puzzleTransform = gAPM.GetPuzzleManager()?.transform;
        
        // Nesnenin başlangıç pozisyonunu ayarla
        if (puzzleTransform != null)
        {
            Vector3 worldPos = GetMouseWorldPosition(eventData);
            
            // PuzzleDirection'a göre pozisyonu ayarla
            PuzzleDirection direction = gAPM.GetPuzzleManager().direction;
            AdjustObjectBasedOnPuzzleDirection(worldPos, direction);
            
            Debug.Log($"Begin drag with direction: {direction}");
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item == null || puzzleTransform == null) return;
        
        // Mouse'un dünya pozisyonunu al
        Vector3 worldPos = GetMouseWorldPosition(eventData);
        
        // PuzzleDirection'a göre pozisyonu ayarla
        PuzzleDirection direction = gAPM.GetPuzzleManager().direction;
        AdjustObjectBasedOnPuzzleDirection(worldPos, direction);
        
        Debug.Log("Item position: " + item.sceneObject.transform.position);
    }
    
    private void AdjustObjectBasedOnPuzzleDirection(Vector3 mouseWorldPos, PuzzleDirection direction)
    {
        if (item == null || item.sceneObject == null || puzzleTransform == null) return;
        
        Vector3 puzzlePos = puzzleTransform.position;
        Vector3 newPosition = mouseWorldPos;
        Vector3 offsetDirection = Vector3.zero;
        
        // Puzzle yönüne göre pozisyonu ayarla
        switch (direction)
        {
            case PuzzleDirection.x:
                // X eksenine bakan puzzle (Z ekseni sabit)
                // Offset yönü Z ekseni boyunca (puzzle'dan uzaklaşma)
                offsetDirection = Vector3.forward;
                
                // Z pozisyonunu puzzle pozisyonuna offset ekleyerek ayarla
                newPosition.z = puzzlePos.z + puzzleOffset;
                
                // X-Y düzleminde ilerle, rotasyonu X eksenine bakan şekilde ayarla
                item.sceneObject.transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
                break;
                
            case PuzzleDirection.y:
                // Y eksenine bakan puzzle (X ekseni sabit)
                // Offset yönü X ekseni boyunca (puzzle'dan uzaklaşma)
                offsetDirection = Vector3.right;
                
                // X pozisyonunu puzzle pozisyonuna offset ekleyerek ayarla
                newPosition.x = puzzlePos.x + puzzleOffset;
                
                // Y-Z düzleminde ilerle, rotasyonu Y eksenine bakan şekilde ayarla
                item.sceneObject.transform.rotation = Quaternion.LookRotation(Vector3.right, Vector3.up);
                break;
        }
        
        // Puzzle merkezine olan mesafeyi kontrol et (offset hariç)
        // Offset yönünü hesaplamadan önce yatay mesafeyi hesapla
        Vector3 horizontalDiff = newPosition - puzzlePos;
        
        // Offset yönündeki bileşeni çıkar (yatay mesafeyi hesaplamak için)
        if (direction == PuzzleDirection.x)
        {
            horizontalDiff.z = 0; // Z yönünü (derinlik) hesaba katma
        }
        else // PuzzleDirection.y
        {
            horizontalDiff.x = 0; // X yönünü (genişlik) hesaba katma
        }
        
        float horizontalDistance = horizontalDiff.magnitude;
        
        if (horizontalDistance > interactionDistance)
        {
            // Mesafeyi sınırla (sadece yatay düzlemde)
            Vector3 horizontalDir = horizontalDiff.normalized;
            horizontalDiff = horizontalDir * interactionDistance;
            
            // Yeni pozisyonu güncelle, offset korunarak
            if (direction == PuzzleDirection.x)
            {
                newPosition.x = puzzlePos.x + horizontalDiff.x;
                newPosition.y = puzzlePos.y + horizontalDiff.y;
                // Z zaten offset ile ayarlandı
            }
            else // PuzzleDirection.y
            {
                newPosition.y = puzzlePos.y + horizontalDiff.y;
                newPosition.z = puzzlePos.z + horizontalDiff.z;
                // X zaten offset ile ayarlandı
            }
        }
        
        // Debug bilgisi
        Debug.DrawLine(puzzlePos, newPosition, Color.yellow, 0.1f);
        Debug.Log($"Offset direction: {offsetDirection}, Distance: {horizontalDistance}");
        
        // Nesnenin pozisyonunu güncelle
        item.sceneObject.transform.position = newPosition;
    }
    
    private Vector3 GetMouseWorldPosition(PointerEventData eventData)
    {
        // Mouse pozisyonunu al
        Vector3 mousePos = eventData.position;
        
        // Eğer puzzle varsa, z değerini puzzle'ın kameraya olan mesafesi olarak ayarla
        if (puzzleTransform != null)
        {
            // Puzzle'ın ekran pozisyonunu al
            Vector3 puzzleScreenPos = mainCamera.WorldToScreenPoint(puzzleTransform.position);
            mousePos.z = puzzleScreenPos.z;
        }
        else
        {
            // Sabit z değeri kullan
            mousePos.z = zCoordinate;
        }
        
        // Ekran koordinatlarını dünya koordinatlarına çevir
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
        
        return worldPos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (item == null) return;

        if (InventorySystem.Instance.isPuzzleMode && puzzleTransform != null)
        {
            // Nesne ile puzzle arasındaki mesafeyi kontrol et
            float distance = Vector3.Distance(item.sceneObject.transform.position, puzzleTransform.position);
            
            if (distance <= interactionDistance)
            {
                // Nesne puzzle alanına yerleştirildi
                item.sceneObject.SetActive(true);
                InventorySystem.Instance.RemoveItem(item.itemName, 1);
                Debug.Log($"Item {item.itemName} placed successfully at distance: {distance}");
                
                // Eğer puzzle manager'ın bir fonksiyonu varsa çağırabilirsiniz
                // gAPM.GetPuzzleManager().OnItemPlaced(item);
            }
            else
            {
                // Nesne puzzle alanı dışına bırakıldı
                item.sceneObject.SetActive(false);
                Debug.Log("Item dropped too far from puzzle");
            }
        }
        else
        {
            // Puzzle modu değilse, nesneyi gizle
            if (item.sceneObject != null)
            {
                item.sceneObject.SetActive(false);
            }
        }
        
        transform.position = originalPosition;
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