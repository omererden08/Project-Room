using System;
using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
[RequireComponent(typeof(PlayerMovementController))]
public class PlayerController : MonoBehaviour
{
    // References to component systems
    private PlayerInputHandler inputHandler;
    private PlayerMovementController movementController;
    private InteractionSystem interactionSystem;

    
    [SerializeField] private Transform dropPosition;
    
    // Pause state
    private bool _isPaused = false;
    public bool IsPaused
    {
        get => _isPaused;
        private set
        {
            bool oldValue = _isPaused;
            _isPaused = value;
            
            if (oldValue != value)
            {
                UpdatePauseState();
                
                if (_isPaused)
                    OnControllerPaused?.Invoke();
                else
                    OnControllerResumed?.Invoke();
            }
        }
    }
    
    // Events
    public event Action OnControllerPaused;
    public event Action OnControllerResumed;
    
    private void Awake()
    {
        // Get required components
        inputHandler = GetComponent<PlayerInputHandler>();
        movementController = GetComponent<PlayerMovementController>();
        interactionSystem = GetComponent<InteractionSystem>();

        
        // Set drop position if not assigned
        if (dropPosition == null)
        {
            dropPosition = transform;
        }
    }
    

    
    private void HandleInteract()
    {
        if (interactionSystem != null)
        {
            interactionSystem.InteractWithCurrent();
        }
    }
    

    

    // Public methods to pause/resume the controller
    public void PauseController()
    {
        IsPaused = true;
    }
    
    public void ResumeController()
    {
        IsPaused = false;
    }
    
    // Toggle method for convenience
    public void TogglePause()
    {
        IsPaused = !IsPaused;
    }
    
    private void UpdatePauseState()
    {
        // Update pause state in all subsystems
        if (inputHandler != null) inputHandler.IsPaused = IsPaused;
        if (movementController != null) movementController.IsPaused = IsPaused;
        if (interactionSystem != null) interactionSystem.IsPaused = IsPaused;
        
        // Handle cursor lock
        Cursor.lockState = IsPaused ? CursorLockMode.None : CursorLockMode.Locked;
    }
}