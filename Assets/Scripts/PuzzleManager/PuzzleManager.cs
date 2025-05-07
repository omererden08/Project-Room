using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.SceneManagement;

public class PuzzleManager : IInteractable
{
    [Header("Puzzle Camera Settings")]
    [SerializeField] public Transform cameraFocusPoint;
    [SerializeField] private UnityEngine.Vector3 cameraDistance;
    [SerializeField] private float transitionDuration = 1.5f;
    [SerializeField] private Ease cameraEaseType = Ease.InOutSine;

    [Header("Puzzle Logic")]
    [SerializeField] public bool solved = false;
    [SerializeField] private float autoExitDelay = 0.2f;
    [SerializeField] private KeyCode exitPuzzleKey = KeyCode.Escape;

    public PuzzleDirection direction;
    public static PuzzleManager ActivePuzzleManager => activePuzzleManager;

    // References
    private Camera mainCamera;
    private Transform originalCameraParent;
    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;
    public bool inPuzzleMode = false;
    private PlayerController playerController;
    public Transform puzzleRootTransform;

    // Static lock to ensure only one PuzzleManager is active
    private static PuzzleManager activePuzzleManager = null;
    public Material LightMaterial;

    public bool CanSolvable;


    private bool isGameEnded = false;
    private int counter = 0;

    public Collider col;


    private void Start()
    {
        col = GetComponent<Collider>();
        col.enabled = true;
        outline = GetComponent<Outline3D>();
        CanSolvable = true;
        LightMaterial.color = Color.red;
        LightMaterial.EnableKeyword("_EMISSION");
        LightMaterial.SetColor("_EmissionColor", Color.red);
        EvntManager.TriggerEvent("GameMode");
        if (cameraFocusPoint == null)
            cameraFocusPoint = transform;

        mainCamera = Camera.main;

        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        if (playerController == null)
            Debug.LogWarning("PuzzleManager: FirstPersonController not found!");

    }

    private void Update()
    {
        if (inPuzzleMode && Input.GetKeyDown(exitPuzzleKey))
        {
            ExitPuzzle();
        }
        if (isGameEnded)
        {
            EndGame();
        }

    }

    void EndGame()
    {
        isGameEnded = true;

        Debug.Log("Oyun bitti!");
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextSceneIndex);
        Destroy(gameObject);

    }

    public override void Interact()
    {
        base.Interact(); // Temel sınıfın Interact metodunu çağırır

        if (!inPuzzleMode && CanSolvable)
            StartPuzzle();
        else
            ExitPuzzle();
    }

    private void StartPuzzle()
    {
        if (activePuzzleManager != null && activePuzzleManager != this)
        {
            Debug.LogWarning("Another puzzle is already active!");
            return;
        }

        Debug.Log("puzzle solving");
        activePuzzleManager = this;
        col.enabled = false;
        originalCameraParent = mainCamera.transform.parent;
        originalCameraPosition = mainCamera.transform.position;
        originalCameraRotation = mainCamera.transform.rotation;

        playerController?.PauseController();

        UnityEngine.Vector3 directionToCamera = -cameraFocusPoint.forward;
        UnityEngine.Vector3 targetPosition = cameraFocusPoint.position +
                                        directionToCamera + cameraDistance +
                                        UnityEngine.Vector3.up;

        Sequence cameraSequence = DOTween.Sequence();
        mainCamera.transform.SetParent(null);

        Tween moveTween = mainCamera.transform.DOMove(targetPosition, transitionDuration)
            .SetEase(cameraEaseType)
            .OnUpdate(() => mainCamera.transform.LookAt(cameraFocusPoint.position));

        cameraSequence.Append(moveTween);
        cameraSequence.OnComplete(() =>
        {
            mainCamera.transform.LookAt(cameraFocusPoint.position);
            inPuzzleMode = true;
            EnablePuzzleInteraction();
        });
    }

    private void ExitPuzzle()
    {
        if (!inPuzzleMode || activePuzzleManager != this)
            return;

        DisablePuzzleInteraction();

        Sequence cameraSequence = DOTween.Sequence();

        cameraSequence.Append(mainCamera.transform.DOMove(originalCameraPosition, transitionDuration).SetEase(cameraEaseType));
        cameraSequence.Join(mainCamera.transform.DORotateQuaternion(originalCameraRotation, transitionDuration).SetEase(cameraEaseType));

        cameraSequence.OnComplete(() =>
        {
            if (mainCamera != null && originalCameraParent != null)
            {
                mainCamera.transform.SetParent(originalCameraParent);
            }

            playerController?.ResumeController();
            inPuzzleMode = false;
            activePuzzleManager = null;
            col.enabled=true;
        });
    }


    private void EnablePuzzleInteraction()
    {
        EvntManager.TriggerEvent("OpenInventory");

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        outline.enabled = false;
        EvntManager.TriggerEvent("DisableCh");
        EvntManager.TriggerEvent("PuzzleMode");

    }

    private void DisablePuzzleInteraction()
    {
        EvntManager.TriggerEvent("CloseInventory");


        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        outline.enabled = true;
        EvntManager.TriggerEvent("EnableCh");
        EvntManager.TriggerEvent("GameMode");
    }

    public void PuzzleSolved()
    {
        solved = true;
        LightMaterial.color = Color.green;
        LightMaterial.EnableKeyword("_EMISSION");
        LightMaterial.SetColor("_EmissionColor", Color.green);
        CanSolvable = false;
        //end control in here
        Debug.Log("Puzzle solved!");
        counter++;
        if (counter == 2)
        {
            isGameEnded = true;
        }
        StartCoroutine(AutoExitAfterSolve());
    }

    private IEnumerator AutoExitAfterSolve()
    {
        yield return new WaitForSeconds(autoExitDelay);
        ExitPuzzle();
    }

    private void OnDisable()
    {
        if (inPuzzleMode && activePuzzleManager == this)
        {
            ExitPuzzle();
        }
    }
}