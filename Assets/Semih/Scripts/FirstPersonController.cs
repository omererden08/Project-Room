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
    private bool isZooming = false;

    // Physics variables
    private Vector3 velocity;
    private bool isGrounded;

    void Awake()
    {
        // Get components
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        // Get input actions
        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
        jumpAction = playerInput.actions["Jump"];
        zoomAction = playerInput.actions["Zoom"];
        sprintAction = playerInput.actions["Sprint"];

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleZoom();
    }

    void HandleMovement()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        Vector2 movementInput = moveAction.ReadValue<Vector2>();
        Vector3 move = transform.right * movementInput.x + transform.forward * movementInput.y;

        // Use sprint action instead of direct keyboard check
        float currentSpeed = sprintAction.IsPressed() ? sprintSpeed : moveSpeed;
        controller.Move(move.normalized * currentSpeed * Time.deltaTime);

        if (jumpAction.triggered && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleRotation()
    {
        Vector2 lookInput = lookAction.ReadValue<Vector2>() * mouseSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up * lookInput.x);

        xRotation -= lookInput.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void HandleZoom()
    {
        isZooming = zoomAction.IsPressed();
        float targetZoom = isZooming ? zoomFOV : normalFOV;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetZoom, Time.deltaTime * zoomSpeed);
    }

    void OnDestroy()
    {
        Cursor.lockState = CursorLockMode.None;
    }
}