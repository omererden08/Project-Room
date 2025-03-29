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
    private InputAction sprintAction;  // New sprint action

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
    // Interactable variables
    [SerializeField] private Transform holdPos;
    [SerializeField] private bool isRotatingObjects;
    private bool isHeld;

    //inventory
    public Inventory inventory;
    private List<IInteractable> pickedItems = new List<IInteractable>();

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

    private void Start()
    {
        inventory = GameObject.Find("InventoryManager").GetComponent<Inventory>();
    }
    void Update()
    {
        //Rotate();
        if (!isRotatingObjects)
        {
            HandleRotation();
        }
        else
        {
            _lookInput = Vector2.zero;
        }
        if (!isRotatingObjects)
        {
            HandleMovement();
        }

        HandleInteraction();
        Zoom();
        PickUp();
        Drop();

    }
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
        if (CurrentInteractable != null && LatestInteractable == null && !isHeld)
        {
            CurrentInteractable.OutlineShow();
            //optional: outline or glow effect in here, will start
        }
        else if (CurrentInteractable == null && LatestInteractable != null) //tutma kisminda outline show olmuyor ama outline hide oluyor. 
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
    void PickUp()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (CurrentInteractable != null)
            {
                inventory.AddItem(CurrentInteractable.item);
                pickedItems.Add(CurrentInteractable);
                CurrentInteractable.gameObject.SetActive(false);
                print("item geldi");
            }
        }

    }
    void Drop()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            if (LatestInteractable != null)
            {
                IInteractable lastPicked = pickedItems[pickedItems.Count - 1];
                pickedItems.RemoveAt(pickedItems.Count - 1);

                inventory.RemoveItem(lastPicked.item);
                lastPicked.gameObject.SetActive(true);
                print("item gitti");
            }
        }
    }
    //kullanilmiyor suan
    void Rotate()
    {
        Vector2 lookInput = _lookInput * mouseSensitivity * Time.deltaTime;

        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E))
        {
            isRotatingObjects = true;

            if (Input.GetKey(KeyCode.Q) && lookInput.x != 0)
            {
                LatestInteractable.gameObject.transform.Rotate(Vector3.up * lookInput.x);
            }

            if (Input.GetKey(KeyCode.E) && lookInput.y != 0)
            {
                xRotation -= lookInput.y;
                xRotation = Mathf.Clamp(xRotation, -90f, 90f);
                LatestInteractable.gameObject.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            }
        }
        else
        {
            isRotatingObjects = false;
            lookInput = Vector2.zero;
        }
    }



    void OnDestroy()
    {
        Cursor.lockState = CursorLockMode.None;
    }
}  