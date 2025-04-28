using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.SceneManagement;

public class PuzzleManager : IInteractable
{
    [Header("Puzzle Camera Settings")]
    [SerializeField] private Transform cameraFocusPoint;
    [SerializeField] private float cameraDistance = 2f;
    [SerializeField] private float cameraHeight = 1f;
    [SerializeField] private float transitionDuration = 1.5f;
    [SerializeField] private Ease cameraEaseType = Ease.InOutSine;

    [Header("Puzzle Logic")]
    [SerializeField] public bool solved = false;
    [SerializeField] private float autoExitDelay = 0.2f;
    [SerializeField] private KeyCode exitPuzzleKey = KeyCode.Escape;

    [Header("UI Elements")]
    [SerializeField] private GameObject puzzleUI;

    [SerializeField] private GameObject[] gameUI;

    // References
    private Camera mainCamera;
    private Transform originalCameraParent;
    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;
    public bool inPuzzleMode = false;
    private FirstPersonController playerController;

    // Static lock to ensure only one PuzzleManager is active
    private static PuzzleManager activePuzzleManager = null;
    public Material LightMaterial;

    public bool CanSolvable;
    public CheckEnd checkEnd;
    //Ending
    [SerializeField] private CanvasGroup endScreenCanvasGroup;
    [SerializeField] private float fadeDuration = 1f;

    private bool isGameEnded = false;
    private int counter = 0;

    private void Awake()
    {
        endScreenCanvasGroup.alpha = 0f;
        endScreenCanvasGroup = GameObject.Find("CanvasEnd").GetComponent<CanvasGroup>();
    }


    private void Start()
    {
        CanSolvable = true;
        LightMaterial.color = Color.red;
        LightMaterial.EnableKeyword("_EMISSION");
        LightMaterial.SetColor("_EmissionColor", Color.red);
        if (cameraFocusPoint == null)
            cameraFocusPoint = transform;

        mainCamera = Camera.main;

        playerController = GameObject.Find("Player").GetComponent<FirstPersonController>();
        if (playerController == null)
            Debug.LogWarning("PuzzleManager: FirstPersonController not found!");

        if (puzzleUI != null)
            puzzleUI.SetActive(false);

        if (gameUI == null)
            Debug.LogError("Game UI null!");
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
        endScreenCanvasGroup.DOFade(1f, fadeDuration).OnComplete(() =>
        {
            Debug.Log("Oyun bitti!");
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            SceneManager.LoadScene(nextSceneIndex);
            Destroy(gameObject);
        });
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
        activePuzzleManager = this;

        originalCameraParent = mainCamera.transform.parent;
        originalCameraPosition = mainCamera.transform.position;
        originalCameraRotation = mainCamera.transform.rotation;

        playerController?.PauseController();

        Vector3 directionToCamera = -cameraFocusPoint.forward;
        Vector3 targetPosition = cameraFocusPoint.position +
                                directionToCamera * cameraDistance +
                                Vector3.up * cameraHeight;

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
        });
    }

    private void EnablePuzzleInteraction()
    {
        if (puzzleUI != null)
            puzzleUI.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        foreach (GameObject obj in gameUI)
        {
            obj.SetActive(false);
        }

    }

    private void DisablePuzzleInteraction()
    {
        if (puzzleUI != null)
            puzzleUI.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        foreach (GameObject obj in gameUI)
        {
            obj.SetActive(true);
        }
    }

    public void PuzzleSolved()
    {
        solved = true;
        LightMaterial.color = Color.green;
        LightMaterial.EnableKeyword("_EMISSION");
        LightMaterial.SetColor("_EmissionColor", Color.green);
        CanSolvable = false;
        checkEnd.Check();
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