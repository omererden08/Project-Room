using UnityEngine;

public class CollectableItem : IInteractable
{
    public Item item;
    public bool inPuzzleMode = false;
    private bool isPickedUp = false; // Aynı nesnenin tekrar eklenmesini önle

    public override void PickUp()
    {
        base.PickUp();
        if (item == null || inPuzzleMode || isPickedUp)
        {
            Debug.LogWarning("PickUp: Geçersiz item, puzzle modu aktif veya nesne zaten alındı");
            return;
        }

        isPickedUp = true;
        bool added = InventorySystem.Instance.AddItem(new Item(item.itemName, item.icon, item.quantity, gameObject));
        if (added)
        {
            gameObject.SetActive(false); // Nesneyi sahnede gizle
        }
        else
        {
            Debug.Log("Envanter dolu, nesne eklenemedi: " + item.itemName);
            isPickedUp = false; // Ekleme başarısızsa sıfırla
        }
    }

    public override void Interact()
    {
        base.Interact();
        if (item == null)
        {
            Debug.LogWarning("Interact: Item null");
            return;
        }
        InventorySystem.Instance.SetPuzzleMode(true);
    }
}