using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    // Player components
    private CharacterController controller;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction zoomAction;
    private InputAction sprintAction;

    // Movement variables
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    // Camera variables
    [SerializeField] private Camera cam;
    [SerializeField] private float mouseSensitivity = 100f;
    private float xRotation = 0f;

    // Zoom variables
    [SerializeField] private float zoomSpeed = 10f;
    // Physics variables
    private Vector3 velocity;
    private bool isGrounded;
    // Inventory variables
    [SerializeField] private Inventory inventory;
    [SerializeField] private List<IInteractable> pickedUpItems = new List<IInteractable>();

    //inputs
    public bool cursorInputForLook = true;
    private Vector2 _movementInput;
    private Vector2 _lookInput;
    private bool isZooming;
    public float minZoom = 40f;
    public float maxZoom = 60f;
    private float targetZoom = 60f;
    private bool isSprinting;
    public float interactionDistance;
    public float scrollValue;
    public LayerMask interactableLayer;
    private IInteractable _currentInteractable;
    private IInteractable _latestInteractable;

    // Controller state variables
    [SerializeField] private bool _isPaused = false;
    public bool IsPaused
    {
        get => _isPaused;
        private set
        {
            bool oldValue = _isPaused;
            _isPaused = value;

            // Fire event if value changed
            if (oldValue != value)
            {
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

    public IInteractable CurrentInteractable
    {
        get => _currentInteractable;
        set
        {
            if (_currentInteractable != value)
            {
                _latestInteractable = _currentInteractable;
                _currentInteractable = value;
                InteractChanged();
            }
        }
    }
    public IInteractable LatestInteractable
    {
        get => _latestInteractable;
        set => _latestInteractable = value;
    }

    void Awake()
    {
        // Get components
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        inventory = GameObject.Find("InventoryManager").GetComponent<Inventory>();
        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
    }

    #region Inputs
    void OnMove(InputValue value)
    {
        _movementInput = value.Get<Vector2>();
    }
    void OnLook(InputValue value)
    {
        if (cursorInputForLook)
        {
            _lookInput = value.Get<Vector2>();
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
    void OnZoom(InputValue value)
    {
        isZooming = value.isPressed;
    }
    void OnSprint(InputValue value)
    {
        isSprinting = value.isPressed;
    }
    #endregion

    void Update()
    {
        // Skip most functionality if paused
        if (!IsPaused)
        {
            HandleMovement();
            HandleRotation();
            HandleInteraction();
            Zoom();
            PickUp();
            Drop();
        }
    }

    void HandleMovement()
    {
        // Skip if paused
        if (IsPaused) return;

        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        Vector3 move = transform.right * _movementInput.x + transform.forward * _movementInput.y;

        // Use sprint action instead of direct keyboard check
        float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;
        controller.Move(move.normalized * currentSpeed * Time.deltaTime);
        controller.Move(velocity * Time.deltaTime);

        if(transform.position.y > 4)
        {
            transform.position = new Vector3(transform.position.x, 4, transform.position.z);
        }

    }

    void HandleRotation()
    {
        // Skip if paused
        if (IsPaused) return;

        Vector2 lookInput = _lookInput * mouseSensitivity * Time.deltaTime;

        if (lookInput.x != 0)
        {
            transform.Rotate(Vector3.up * lookInput.x);
        }

        if (lookInput.y != 0)
        {
            xRotation -= lookInput.y;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }

    void HandleInteraction()
    {
        // Skip if paused
        if (IsPaused) return;

        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance, interactableLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                CurrentInteractable = interactable;
                // Optional: Add UI hint or crosshair change here
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

    void InteractChanged()
    {
        if (CurrentInteractable != null && LatestInteractable == null)
        {
            CurrentInteractable.OutlineShow();
            //optional: outline or glow effect in here, will start
        }
        else if (CurrentInteractable == null && LatestInteractable != null)
        {
            LatestInteractable.OutlineHide();
            //optional: outline or glow effect in here, will stop        
        }
    }

    void Zoom()
    {
        // Skip if paused
        if (IsPaused) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        targetZoom -= scroll * zoomSpeed;
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetZoom, Time.deltaTime * zoomSpeed);
    }

    void PickUp()
    {
        // Skip if paused
        if (IsPaused) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (CurrentInteractable != null)
            {
                inventory.AddItem(CurrentInteractable.item);
                pickedUpItems.Add(CurrentInteractable);
                CurrentInteractable.gameObject.SetActive(false);
            }

        }
    }
    void Drop()
    {
        // Skip if paused
        if (IsPaused) return;

        if (Input.GetKeyDown(KeyCode.U))
        {

            if (pickedUpItems.Count > 0)
            {
                IInteractable dropItems = pickedUpItems[pickedUpItems.Count - 1]; // En son alýnan
                pickedUpItems.RemoveAt(pickedUpItems.Count - 1); // Listeden çýkar

                inventory.RemoveItem(dropItems.item);
                dropItems.gameObject.SetActive(true);
            }


        }
    }

    // Public methods to pause/resume the controller
    public void PauseController()
    {
        // Store initial input state or reset them if needed
        _movementInput = Vector2.zero;
        _lookInput = Vector2.zero;

        IsPaused = true;
    }

    public void ResumeController()
    {
        IsPaused = false;
    }

    // Toggle method for convenience
    public void TogglePause()
    {
        if (IsPaused)
            ResumeController();
        else
            PauseController();
    }

    void OnDestroy()
    {
        Cursor.lockState = CursorLockMode.None;
    }
}