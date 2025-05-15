using UnityEngine;

public class CollectableItem : IInteractable
{
    public Item item;
    public bool inPuzzleMode = false;
    private bool isPickedUp = false;
    void Start()
    {
        bomb = FindAnyObjectByType<Bomb>();
        outline = GetComponent<Outline3D>();
        outline.enabled = false;
    }

    public override void PickUp()
    {
        base.PickUp();

        if (!bomb.isBoombReady)
        {
            EvntManager.TriggerEvent("subID", "STRT_INTER_" + Random.Range(1, 5));
            return;
        }
        if (item == null || inPuzzleMode || isPickedUp)
        {
            Debug.LogWarning("PickUp: Geçersiz item, puzzle modu aktif veya nesne zaten alındı");
            return;
        }

        isPickedUp = true;
        bool added = InventorySystem.Instance.AddItem(new Item(item.itemName, item.icon, item.outlinedIcon, item.quantity, gameObject));
        if (added)
        {
            gameObject.SetActive(false);
            Debug.Log("Nesne alındı: " + item.itemName);
        }
        else
        {
            Debug.Log("Envanter dolu, nesne eklenemedi: " + item.itemName);
            isPickedUp = false;
        }
    }

    public override void Interact()
    {
        base.Interact();

        if (!bomb.isBoombReady)
        {
            EvntManager.TriggerEvent("subID", "STRT_INTER_" + Random.Range(1, 5));
            return;
        }

        if (item == null)
        {
            Debug.LogWarning("Interact: Item null");
            return;
        }
        InventorySystem.Instance.SetPuzzleMode(true);
    }
}