using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler
{
    public Item item;
    public Image image;
    public TMPro.TMP_Text quantityText;
    void Start()
    {
        quantityText = GetComponentInChildren<TMPro.TMP_Text>();
    }

    public void SetItem(Item newItem, Sprite emptySprite, int quantity)
    {
        item = newItem;

        if (image == null)
            image = GetComponent<Image>();

        if (item != null)
        {
            image.sprite = item.itemSprite;
            image.color = Color.white;
            quantityText.text = quantity.ToString();
        }
        else
        {
            image.sprite = emptySprite;
            image.color = new Color(1, 1, 1, 0.3f);
        }
    }
    public void SetItem(Item newItem, Sprite emptySprite)
    {
        item = newItem;

        if (image == null)
            image = GetComponent<Image>();

        if (item != null)
        {
            image.sprite = item.itemSprite;
            image.color = Color.white;
        }
        else
        {
            image.sprite = emptySprite;
            image.color = new Color(1, 1, 1, 0.3f);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (item != null && item.itemPrefab != null)
        {
            Vector3 spawnPos = GetMouseWorldPosition();
            Instantiate(item.itemPrefab, spawnPos, Quaternion.identity);
            Debug.Log($"Instantiate edildi: {item.itemName}");
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            return hit.point;
        }
        return ray.GetPoint(5f); // E�er raycast bo�a giderse
    }
}
