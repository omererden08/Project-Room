using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.Analytics;

public class PuzzleManager : IInteractable
{
    [Header("Puzzle Camera Settings")]
    [SerializeField] private Transform cameraFocusPoint;
    [SerializeField] private float cameraDistance = 2f;
    [SerializeField] private float cameraHeight = 1f;
    [SerializeField] private float transitionDuration = 1.5f;
    [SerializeField] private Ease cameraEaseType = Ease.InOutSine;

    [Header("Puzzle Logic")]
    [SerializeField] private bool solved = false;
    [SerializeField] private float autoExitDelay = 2f; // Time to wait after solving before exiting
    [SerializeField] private KeyCode exitPuzzleKey = KeyCode.Escape;

    [Header("UI Elements")]
    [SerializeField] private GameObject puzzleUI; // Optional UI for the puzzle

    // References
    private Camera mainCamera;
    private Transform originalCameraParent;
    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;
    private bool inPuzzleMode = false;

    // Player controller reference
    private FirstPersonController playerController;

    private void Start()
    {
        if (cameraFocusPoint == null)
            cameraFocusPoint = transform;

        mainCamera = Camera.main;

        // Find player controller
        playerController = FindObjectOfType<FirstPersonController>();
        if (playerController == null)
            Debug.LogWarning("PuzzleManager: FirstPersonController not found!");

        // Hide puzzle UI on start if there is one
        if (puzzleUI != null)
            puzzleUI.SetActive(false);
    }

    private void Update()
    {
        // Allow manual exit with escape key
        if (inPuzzleMode && Input.GetKeyDown(exitPuzzleKey))
        {
            ExitPuzzle();
        }
    }

    public override void Interact()
    {
        base.Interact();

        if (!inPuzzleMode)
            StartPuzzle();
        else
            ExitPuzzle();
    }
    private void StartPuzzle()
{
    // Store original camera state
    originalCameraParent = mainCamera.transform.parent;
    originalCameraPosition = mainCamera.transform.position;
    originalCameraRotation = mainCamera.transform.rotation;
    
    // Pause player controller
    playerController?.PauseController();
    
    // Calculate position directly in front of the puzzle
    Vector3 directionToCamera = -cameraFocusPoint.forward;
    Vector3 targetPosition = cameraFocusPoint.position + 
                            directionToCamera * cameraDistance + 
                            Vector3.up * cameraHeight;
    
    // Animate camera to puzzle view
    Sequence cameraSequence = DOTween.Sequence();
    mainCamera.transform.SetParent(null);
    
    // Move to position while continuously looking at target
    Tween moveTween = mainCamera.transform.DOMove(targetPosition, transitionDuration)
        .SetEase(cameraEaseType)
        .OnUpdate(() => {
            mainCamera.transform.LookAt(cameraFocusPoint.position);
        });
    
    cameraSequence.Append(moveTween);
    
    cameraSequence.OnComplete(() => {
        mainCamera.transform.LookAt(cameraFocusPoint.position); // Final precise adjustment
        inPuzzleMode = true;
        EnablePuzzleInteraction();
    });
}
    private void ExitPuzzle()
    {
        DisablePuzzleInteraction();

        // Animate camera back to original position
        Sequence cameraSequence = DOTween.Sequence();

        cameraSequence.Append(mainCamera.transform.DOMove(originalCameraPosition, transitionDuration).SetEase(cameraEaseType));
        cameraSequence.Join(mainCamera.transform.DORotateQuaternion(originalCameraRotation, transitionDuration).SetEase(cameraEaseType));

        cameraSequence.OnComplete(() =>
        {
            // Restore camera parent
            if (originalCameraParent != null)
            {
                mainCamera.transform.SetParent(originalCameraParent);
            }

            // Resume player movement
            playerController?.ResumeController();

            inPuzzleMode = false;
        });
    }

    // Enable puzzle-specific interactions
    private void EnablePuzzleInteraction()
    {
        // Show puzzle UI
        if (puzzleUI != null)
            puzzleUI.SetActive(true);

        // Enable cursor for puzzle interactions
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Disable puzzle-specific interactions
    private void DisablePuzzleInteraction()
    {
        // Hide puzzle UI
        if (puzzleUI != null)
            puzzleUI.SetActive(false);

        // Lock cursor again for FPS controls
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Call this when puzzle is solved
    public void PuzzleSolved()
    {
        solved = true;
        Debug.Log("Puzzle solved!");

        // Auto exit after delay
        StartCoroutine(AutoExitAfterSolve());
    }

    private IEnumerator AutoExitAfterSolve()
    {
        yield return new WaitForSeconds(autoExitDelay);
        ExitPuzzle();
    }

    // Optional: Listen for player controller events
    private void OnEnable()
    {
        if (playerController != null)
        {
            playerController.OnControllerPaused += OnPlayerControllerPaused;
            playerController.OnControllerResumed += OnPlayerControllerResumed;
        }
    }

    private void OnDisable()
    {
        if (playerController != null)
        {
            playerController.OnControllerPaused -= OnPlayerControllerPaused;
            playerController.OnControllerResumed -= OnPlayerControllerResumed;
        }
    }

    // Event callbacks
    private void OnPlayerControllerPaused()
    {
        // You can respond to player controller being paused by other systems
        Debug.Log("Player controller was paused by another system");
    }

    private void OnPlayerControllerResumed()
    {
        // You can respond to player controller being resumed by other systems
        Debug.Log("Player controller was resumed by another system");

        // If we're in puzzle mode and player was resumed by another system,
        // we might want to exit puzzle mode
        if (inPuzzleMode)
        {
            ExitPuzzle();
        }
    }
}