using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;

public class InventoryAnimation : MonoBehaviour
{
    public RectTransform InventoryUI;
    public RectTransform ClosedPosition;
    public RectTransform OpenedPosition;
    public float animationDuration;
    public bool isOpen = true;
    public bool inAnimation;
    public void Start()
    {
        if (InventoryUI == null) { Debug.LogError("InventoryAnimation: InventoryUI is null"); }
        if (ClosedPosition == null) { Debug.LogError("InventoryAnimation: ClosedPosition is null"); }
        if (OpenedPosition == null) { Debug.LogError("InventoryAnimation: OpenedPosition is null"); }

        ToClosedPosition();
        EvntManager.StartListening("OpenInventory", ToOpenPosition);
        EvntManager.StartListening("CloseInventory", ToClosedPosition);
    }

    public void ToClosedPosition()
    {
        InventoryUI.DOAnchorPos(ClosedPosition.anchoredPosition, animationDuration).OnUpdate(() => inAnimation = true).OnComplete(() => { isOpen = false; inAnimation = false; });
    }
    public void ToOpenPosition()
    {
        InventoryUI.DOAnchorPos(OpenedPosition.anchoredPosition, animationDuration).OnUpdate(() => inAnimation = true).OnComplete(() => { isOpen = true; inAnimation = false; });
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (isOpen && !inAnimation)
            {
                ToClosedPosition();
            }
            else if (!inAnimation)
            {
                ToOpenPosition();
            }
        }
    }
}
