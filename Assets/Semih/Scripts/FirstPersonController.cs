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
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -9.81f;

    // Camera variables
    [SerializeField] private Camera cam;
    [SerializeField] private float mouseSensitivity = 100f;
    private float xRotation = 0f;

    // Zoom variables
    [SerializeField] private float normalFOV = 60f;
    [SerializeField] private float zoomFOV = 30f;
    [SerializeField] private float zoomSpeed = 10f;
    // Physics variables
    private Vector3 velocity;
    private bool isGrounded;
    //inputs
    public bool cursorInputForLook = true;
    private Vector2 _movementInput;
    private Vector2 _lookInput;
    private bool isZooming;
    private bool isSprinting;
    public float interactionDistance;
    public LayerMask interactableLayer;
    public IInteractable currentInteractable;   

    void Awake()
    {
        // Get components
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        //diger yol bu noktada daha mantikli geldi
        jumpAction = playerInput.actions["Jump"];
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
        if (currentInteractable != null)
        {
            currentInteractable.Interact();
            Debug.Log("Interacting with: " + currentInteractable);
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
        HandleZoom();
        HandleInteraction();
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

        if (jumpAction.triggered && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    void HandleZoom()
    {
        float targetZoom = isZooming ? zoomFOV : normalFOV;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetZoom, Time.deltaTime * zoomSpeed);
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
                currentInteractable = interactable;
                // Optional: Add UI hint or crosshair change here
            }
            else
            {
                currentInteractable = null;
            }
        }
        else
        {
            currentInteractable = null;
        }
    }
    void OnDestroy()
    {
        Cursor.lockState = CursorLockMode.None;
    }
}