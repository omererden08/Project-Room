using UnityEngine;

[System.Serializable]
public class Item
{
    public string itemName;
    public Sprite icon;
    public int quantity;
    public GameObject sceneObject; // Sahnede önceden yerleştirilmiş nesne

    public Item(string name, Sprite icon, int qty, GameObject sceneObject = null)
    {
        this.itemName = name;
        this.icon = icon;
        this.quantity = qty;
        this.sceneObject = sceneObject;
    }
}