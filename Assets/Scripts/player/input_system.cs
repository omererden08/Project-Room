using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInput playerInput;
    
    // Input values
    private Vector2 movementInput;
    private Vector2 lookInput;

    private float scrollValue;
    
    // Input properties
    public Vector2 MovementInput => movementInput;
    public Vector2 LookInput => lookInput;
    // Settings
    public bool cursorInputForLook = true;
    
    // Events
    public event Action OnInteractPressed;
    public event Action OnPickupPressed;
    public event Action OnDropPressed;
    public event Action OnPauseToggled;
    
    private bool _isPaused = false;
    public bool IsPaused
    {
        get => _isPaused;
        set
        {
            bool oldValue = _isPaused;
            _isPaused = value;
            
            if (oldValue != value)
            {
                if (_isPaused)
                {
                    // Reset inputs when paused
                    movementInput = Vector2.zero;
                    lookInput = Vector2.zero;
                }
            }
        }
    }
    
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }
    
    private void Start()
    {
        // Lock cursor at start
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    private void OnDestroy()
    {
        // Unlock cursor when destroyed
        Cursor.lockState = CursorLockMode.None;
    }
    
    // Input System callbacks
    public void OnMove(InputValue value)
    {
        if (!IsPaused)
        {
            movementInput = value.Get<Vector2>();
        }
    }
    
    public void OnLook(InputValue value)
    {
        if (!IsPaused && cursorInputForLook)
        {
            lookInput = value.Get<Vector2>();
        }
    }
    
    public void OnInteract(InputValue value)
    {
        if (!IsPaused && value.isPressed)
        {
            OnInteractPressed?.Invoke();
        }
    }
    

    public void OnPickup(InputValue value)
    {
        if (!IsPaused && value.isPressed)
        {
            OnPickupPressed?.Invoke();
        }
    }
    
    public void OnDrop(InputValue value)
    {
        if (!IsPaused && value.isPressed)
        {
            OnDropPressed?.Invoke();
        }
    }
    
    public void OnPause(InputValue value)
    {
        if (value.isPressed)
        {
            IsPaused = !IsPaused;
            OnPauseToggled?.Invoke();
        }
    }
    
    public void OnScroll(InputValue value)
    {
        if (!IsPaused)
        {
            scrollValue = value.Get<float>();
        }
    }
    
    // Method to manually clear all inputs (useful for pause states)
    public void ClearInputs()
    {
        movementInput = Vector2.zero;
        lookInput = Vector2.zero;
        scrollValue = 0f;
    }
}