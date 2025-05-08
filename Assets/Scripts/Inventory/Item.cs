using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Item
{
    public string itemName;
    public Sprite icon;
    public int quantity;
    public List<GameObject> sceneObjects; // Her item için birden fazla sceneObject tutar

    public Item(string name, Sprite icon, int qty, GameObject obj)
    {
        itemName = name;
        this.icon = icon;
        quantity = qty;
        sceneObjects = new List<GameObject>();
        if (obj != null)
        {
            obj.SetActive(false); // Nesneyi envantere eklerken gizle
            sceneObjects.Add(obj);
        }
    }
    public void FalseAll()
    {
        foreach (GameObject obj in sceneObjects)
        {
            obj.SetActive(false);
        }
    }
    public int GetActiveItemCount()
    {
        return sceneObjects.Count;
    }

}