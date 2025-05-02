using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementController : MonoBehaviour
{
    [Header("Components")]
    private CharacterController controller;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [SerializeField] private float playerHeight = 1.5f;

    [Header("Camera Settings")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float mouseSensitivity = 100f;


    // Internal movement variables
    private Vector3 velocity;
    private float xRotation = 0f;
    private float targetZoom = 60f;
    private bool isGrounded;

    // Reference to the input handler
    private PlayerInputHandler inputHandler;

    private bool _isPaused = false;
    public bool IsPaused
    {
        get => _isPaused;
        set => _isPaused = value;
    }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        inputHandler = GetComponent<PlayerInputHandler>();

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
            if (playerCamera == null)
            {
                Debug.LogError("PlayerMovementController: No camera assigned and cannot find main camera!");
            }
        }
    }

    private void Start()
    {
        // Initialize zoom
        if (playerCamera != null)
        {
            playerCamera.fieldOfView = targetZoom;
        }
    }

    private void Update()
    {
        if (!IsPaused)
        {
            HandleMovement();
            HandleRotation();
            MaintainPlayerHeight();
        }
    }

    private void HandleMovement()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        Vector2 move = inputHandler.MovementInput;
        Vector3 moveDirection = transform.right * move.x + transform.forward * move.y;

        float currentSpeed = moveSpeed;
        controller.Move(moveDirection.normalized * currentSpeed * Time.deltaTime);

        // Apply gravity
        velocity.y += Physics.gravity.y * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleRotation()
    {
        Vector2 look = inputHandler.LookInput * mouseSensitivity * Time.deltaTime;

        if (look.x != 0)
        {
            transform.Rotate(Vector3.up * look.x);
        }

        if (look.y != 0 && playerCamera != null)
        {
            xRotation -= look.y;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }


    private void MaintainPlayerHeight()
    {
        // Maintain consistent player height if needed
        if (Math.Abs(transform.position.y - playerHeight) > 0.01f)
        {
            transform.position = new Vector3(transform.position.x, playerHeight, transform.position.z);
        }
    }

    // Public methods for external control
    public void SetPaused(bool paused)
    {
        IsPaused = paused;
    }

    public void SetPosition(Vector3 position)
    {
        controller.enabled = false;
        transform.position = position;
        controller.enabled = true;
    }

    public void ResetVelocity()
    {
        velocity = Vector3.zero;
    }
}