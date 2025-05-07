using UnityEngine;

public class CollectableItem : IInteractable
{
    public Item item;
    public override void PickUp()
    {
        base.PickUp();
        if (item != null)
        {
            bool added = InventorySystem.Instance.AddItem(new Item(item.itemName, item.icon, item.quantity, gameObject));
            if (added)
            {
                // Nesne envantere eklendi, ek işlem gerekirse buraya eklenebilir
            }
            else
            {
                Debug.Log("Envanter dolu, nesne eklenemedi: " + item.itemName);
            }
        }
    }

    public override void Interact()
    {
        base.Interact();
        InventorySystem.Instance.SetPuzzleMode(true); // Örnek: Etkileşim puzzle modunu başlatır
    }
}