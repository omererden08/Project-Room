using System;
using System.Collections.Generic;
using UnityEngine;


public class InteractionSystem : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private LayerMask interactableLayer;

    // Events
    public event Action<IInteractable> OnInteractableChanged;

    // Properties
    private IInteractable _currentInteractable;
    private IInteractable _latestInteractable;

    // Inventory variables
    [SerializeField] private Inventory inventory;
    [SerializeField] private List<IInteractable> pickedUpItems = new List<IInteractable>();
    int selectedIndex = 0;
    [SerializeField] private float scrollSpeed = 10f;



    public IInteractable CurrentInteractable
    {
        get => _currentInteractable;
        private set
        {
            if (_currentInteractable != value)
            {
                _latestInteractable = _currentInteractable;
                _currentInteractable = value;
                InteractableChanged();
                OnInteractableChanged?.Invoke(_currentInteractable);
            }
        }
    }

    private bool _isPaused = false;
    public bool IsPaused
    {
        get => _isPaused;
        set => _isPaused = value;
    }
    private void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
            if (playerCamera == null)
            {
                Debug.LogError("InteractionSystem: No camera assigned and cannot find main camera!");
            }
        }
        if (inventory == null)
        {
            inventory = FindObjectOfType<Inventory>();
            if (inventory == null)
            {
                Debug.LogError("InteractionSystem: No Inventory found in the scene!");
            }
        }

    }

    private void FixedUpdate()
    {
        if (!IsPaused)
        {
            CheckForInteractable();
            SelectItem(scrollSpeed);
            PickUpItem();
        }
    }

    public void CheckForInteractable()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        IInteractable interactable = null;
        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            interactable = hit.collider.GetComponent<IInteractable>();

        }

        if (interactable != null)
        {
            CurrentInteractable = interactable;
        }
        else
        {
            CurrentInteractable = null;
        }
    }
    void OnInteract()
    {
        // Skip interaction if controller is paused
        if (IsPaused) return;

        Debug.Log("Interacting...");
        if (CurrentInteractable != null)
        {
            CurrentInteractable.Interact();
            Debug.Log("Interacting with: " + CurrentInteractable);
        }
        else
        {
            Debug.Log("Nothing to interact with");
        }
    }
    public void InteractWithCurrent()
    {
        if (IsPaused) return;

        if (CurrentInteractable != null)
        {
            CurrentInteractable.Interact();
            Debug.Log("Interacting with: " + CurrentInteractable);
        }
        else
        {
            Debug.Log("Nothing to interact with");
        }
    }

    private void InteractableChanged()
    {
        if (CurrentInteractable != null && _latestInteractable == null)
        {
            CurrentInteractable.OutlineShow();
            // Optional: Trigger UI changes or other feedback
        }
        else if (CurrentInteractable == null && _latestInteractable != null)
        {
            _latestInteractable.OutlineHide();
            // Optional: Reset UI or other feedback
        }
    }

    void PickUpItem()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                GameObject clickedObject = hit.collider.gameObject;

                if (clickedObject.CompareTag("InventoryItem"))
                {
                    IInteractable interactable = clickedObject.GetComponent<IInteractable>();
                    if (interactable != null)
                    {
                        inventory.AddItem(interactable.item);
                        pickedUpItems.Add(interactable);
                        clickedObject.SetActive(false);
                        Debug.Log("Item picked up: " + interactable.item.itemName);
                    }
                    else
                    {
                        Debug.LogWarning("T�klanan obje 'InventoryItem' tag'li ama InteractableItem scripti yok!");
                    }
                }
            }
        }
    }


    void SelectItem(float scroll)  //select item
    {
        if (IsPaused) return;
        if (pickedUpItems == null || pickedUpItems.Count == 0) return;

        if (scroll > 0f)
        {
            selectedIndex--;
        }
        else if (scroll < 0f)
        {
            selectedIndex++;
        }

        selectedIndex = Mathf.Clamp(selectedIndex, 0, pickedUpItems.Count - 1);

        Debug.Log("Selected Item: " + pickedUpItems[selectedIndex].name);
    }

}