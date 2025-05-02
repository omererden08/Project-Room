using System;
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

    }
    
    private void Update()
    {
        if (!IsPaused)
        {
            CheckForInteractable();
        }
    }
    
    public void CheckForInteractable()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, interactionDistance, interactableLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            
            if (interactable != null)
            {
                CurrentInteractable = interactable;
                // Optional: UI hint could be triggered here or via the event
            }
            else
            {
                CurrentInteractable = null;
            }
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
    
    // Public methods for inventory interactions
    public bool PickUpCurrentInteractable(Inventory inventory)
    {
        if (IsPaused || CurrentInteractable == null) return false;
        
        bool added = inventory.AddItem(CurrentInteractable.item);
        if (added)
        {
            CurrentInteractable.gameObject.SetActive(false);
            return true;
        }
        return false;
    }
}