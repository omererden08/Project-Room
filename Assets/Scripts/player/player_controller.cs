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

    [Header("Sounds")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] walkSound;

    [SerializeField] private Transform dropPosition;

    Vector3 lastPosition;
    bool isMoving;
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
        audioSource = GetComponent<AudioSource>();
        inputHandler = GetComponent<PlayerInputHandler>();
        movementController = GetComponent<PlayerMovementController>();
        interactionSystem = GetComponent<InteractionSystem>();
        lastPosition = transform.position;


        // Set drop position if not assigned
        if (dropPosition == null)
        {
            dropPosition = transform;
        }
    }

    private void Update()
    {
        HandleSound();
    }

    private void HandleInteract()
    {
        if (interactionSystem != null)
        {
            interactionSystem.InteractWithCurrent();
        }
    }

    void HandleSound()
    {
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);

        // E�er hareket ediyorsa
        if (distanceMoved > 0.01f)
        {
            if (!isMoving)
            {
                isMoving = true;

                if (audioSource != null && walkSound.Length > 0)
                {
                    int randomIndex = UnityEngine.Random.Range(0, walkSound.Length);
                    audioSource.clip = walkSound[randomIndex];
                    audioSource.loop = true;
                    audioSource.volume = 0.2f;
                    audioSource.Play();
                }
            }
        }
        else
        {
            if (isMoving)
            {
                isMoving = false;

                if (audioSource != null)
                {
                    audioSource.Stop();
                }
            }
        }

        lastPosition = transform.position;
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