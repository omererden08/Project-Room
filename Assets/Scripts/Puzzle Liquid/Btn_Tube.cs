using Unity.VisualScripting;
using UnityEngine;

public class Btn_Tube : MonoBehaviour
{
    public PuzzleLiquid puzzleLiquid;
    public PuzzleManager puzzleManager;
    public Tube tube;
    public Outline3D outline;
    public ButtonMover buttonMover; // Reference to ButtonMover
    private bool isChosen = false;

    private bool puzzleModeActive;

    void Awake()
    {
        EvntManager.StartListening("GameMode", GameMode);
        EvntManager.StartListening("PuzzleMode", PuzzleMode);
    }

    void Start()
    {
        // Initialize components
        outline = GetComponent<Outline3D>();
        tube = GetComponentInParent<Tube>();
        puzzleLiquid = FindObjectOfType<PuzzleLiquid>();
        buttonMover = GetComponent<ButtonMover>(); // Get ButtonMover on the same GameObject
        outline.enabled = false;

        // Debug: Verify component assignments
        if (buttonMover == null)
        {
            Debug.LogError($"ButtonMover is null on {gameObject.name}");
        }
        if (tube == null)
        {
            Debug.LogError($"Tube is null on {gameObject.name}");
        }
    }

    public void OnMouseEnter()
    {
        if (puzzleModeActive)
        {
            outline.enabled = true;
            outline.OutlineColor = isChosen ? Color.green : Color.white;
        }
    }

    public void OnMouseExit()
    {
        if (puzzleModeActive)
        {
            if (!isChosen)
            {
                outline.enabled = false;
            }
            else
            {
                outline.enabled = true;
                outline.OutlineColor = Color.green;
            }
        }
    }

    public void OnMouseDown()
    {
        if (puzzleModeActive)
        {
            if (puzzleLiquid.chosenTube == null)
            {
                // First click: Set as chosen tube
                puzzleLiquid.SetChosen(tube);
                isChosen = true;
                outline.enabled = true;
                outline.OutlineColor = Color.green;
                if (buttonMover != null)
                {
                    buttonMover.MoveToTarget();
                    Debug.Log($"Moved {gameObject.name}'s ButtonMover to target position (chosen tube)");
                }
                Debug.Log($"Selected {gameObject.name} as chosen tube");
            }
            else if (puzzleLiquid.chosenTube != null && puzzleLiquid.targetTube == null)
            {
                // Second click: Set as target tube and attempt transfer
                Debug.Log($"Target tube set to {gameObject.name}");
                puzzleLiquid.SetTarget(tube);
                if (buttonMover != null)
                {
                    buttonMover.MoveToTarget();
                    Debug.Log($"Moved {gameObject.name}'s ButtonMover to target position (target tube)");
                }
                puzzleLiquid.TransferLiquid(puzzleLiquid.chosenTube, puzzleLiquid.targetTube);
                Debug.Log($"Transferring liquid from {puzzleLiquid.chosenTube.gameObject.name} to {puzzleLiquid.targetTube.gameObject.name}");
                // Reset both tubes
                ResetSelection();
                ResetTargetTube();
                puzzleLiquid.SetChosen(null);
                puzzleLiquid.SetTarget(null);
                EvntManager.TriggerEvent("ApplyLitre");
                Debug.Log("Completed liquid transfer and reset tubes");
            }
        }
    }

    // Resets the selection state of the chosen tube
    private void ResetSelection()
    {
        if (isChosen)
        {
            isChosen = false;
            outline.enabled = false;
            outline.OutlineColor = Color.white;
            if (buttonMover != null)
            {
                buttonMover.MoveToInitial();
                Debug.Log($"Reset {gameObject.name}'s ButtonMover to initial position (chosen tube)");
            }
        }

        // Reset other chosen tubes (in case of multiple selections)
        foreach (var btn in FindObjectsOfType<Btn_Tube>())
        {
            if (btn != this && btn.isChosen)
            {
                btn.isChosen = false;
                btn.outline.enabled = false;
                btn.outline.OutlineColor = Color.white;
                if (btn.buttonMover != null)
                {
                    btn.buttonMover.MoveToInitial();
                    Debug.Log($"Reset {btn.gameObject.name}'s ButtonMover to initial position (other chosen tube)");
                }
            }
        }
    }

    // Resets the target tube's ButtonMover state
    private void ResetTargetTube()
    {
        if (puzzleLiquid.targetTube == null)
        {
            Debug.LogWarning("Target tube is null during reset");
            return;
        }

        foreach (var btn in FindObjectsOfType<Btn_Tube>())
        {
            if (btn.tube == puzzleLiquid.targetTube)
            {
                btn.isChosen = false;
                btn.outline.enabled = false;
                btn.outline.OutlineColor = Color.white;
                if (btn.buttonMover != null)
                {
                    btn.buttonMover.MoveToInitial();
                    Debug.Log($"Reset {btn.gameObject.name}'s ButtonMover to initial position (target tube)");
                }
                else
                {
                    Debug.LogWarning($"ButtonMover is null on target tube {btn.gameObject.name}");
                }
            }
        }
    }

    private void GameMode()
    {
        puzzleModeActive = false;
        gameObject.GetComponent<Collider>().enabled = false;
    }

    private void PuzzleMode()
    {
        puzzleModeActive = true;
        gameObject.GetComponent<Collider>().enabled = true;
    }
}