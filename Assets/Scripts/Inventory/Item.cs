using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite itemSprite;
    public bool isStackable = true;
    public GameObject itemPrefab;
}

[System.Serializable]
public class InventoryItem
{
    public Item item;
    public int quantity = 0;
    public GameObject originalObject; // Reference to the original GameObject

    public void IncreaseQuantity()
    {
        quantity++;
    }

    public InventoryItem(Item item, int quantity, GameObject originalObject)
    {
        this.item = item;
        this.quantity = quantity;
        this.originalObject = originalObject;
    }
}