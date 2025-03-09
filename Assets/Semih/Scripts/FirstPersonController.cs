using System;
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
    private InputAction sprintAction;  // New sprint action

    // Movement variables
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    // Camera variables
    [SerializeField] private Camera cam;
    [SerializeField] private float mouseSensitivity = 100f;
    private float xRotation = 0f;

    [SerializeField] private float zoomSpeed = 10f;
    // Physics variables
    private Vector3 velocity;
    private bool isGrounded;
    //inputs
    public bool cursorInputForLook = true;
    private Vector2 _movementInput;
    private Vector2 _lookInput;
    private bool isZooming;
    public float minZoom = 10f;
    public float maxZoom = 60f;
    private float targetZoom = 60f;
    private bool isSprinting;
    public float interactionDistance;
    public float scrollValue;
    public LayerMask interactableLayer;
    private IInteractable _currentInteractable;
    private IInteractable _latestInteractable;

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
        HandleMovement();
        HandleRotation();
        HandleInteraction();
        Zoom();
    }
    void HandleMovement()
    {
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
    }
    void HandleRotation()
    {
        Vector2 lookInput = _lookInput * mouseSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up * lookInput.x);

        xRotation -= lookInput.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
    void HandleInteraction()
    {
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
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        targetZoom -= scroll * zoomSpeed;
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetZoom, Time.deltaTime * zoomSpeed);
    }
    void OnDestroy()
    {
        Cursor.lockState = CursorLockMode.None;
    }
    void OnRead()
    {
        Debug.Log("OnRead triggered");
        if (CurrentInteractable is Paper paper)
        {
            Debug.Log("Yess");
            Debug.Log(paper.content);
        }
        else
        {
            Debug.Log("IInteractable is not a paper");
        }
    }
}