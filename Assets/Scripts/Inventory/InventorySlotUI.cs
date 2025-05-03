using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler
{
    public InventoryItem inventoryItem;
    public Image image;
    [SerializeField] private TextMeshProUGUI quantityText;


    void Start()
    {
        if (image == null)
            image = GetComponent<Image>();
        if (quantityText == null)
            quantityText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetItem(InventoryItem newInventoryItem, Sprite emptySprite)
    {
        inventoryItem = newInventoryItem;

        if (image == null)
            image = GetComponent<Image>();

        if (inventoryItem != null && inventoryItem.item != null)
        {
            image.sprite = inventoryItem.item.itemSprite;
            image.color = Color.white;

            if (inventoryItem.item.isStackable && inventoryItem.quantity > 1)
            {
                quantityText.text = inventoryItem.quantity.ToString();
            }
            else
            {
                quantityText.text = "";
            }
        }
        else
        {
            image.sprite = emptySprite;
            image.color = new Color(1, 1, 1, 0.3f);
            quantityText.text = "";
        }
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (inventoryItem != null && inventoryItem.item != null && inventoryItem.item.itemPrefab != null)
        {
            Vector3 spawnPos = GetMouseWorldPosition();
            Instantiate(inventoryItem.item.itemPrefab, spawnPos, Quaternion.identity);
            Debug.Log($"Instantiate edildi: {inventoryItem.item.itemName}");
        }
    }


    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            return hit.point;
        }
        return ray.GetPoint(5f); // Eðer raycast boþa giderse
    }
}
