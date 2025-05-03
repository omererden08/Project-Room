using Unity.VisualScripting;
using UnityEngine;

public class Btn_Tube : MonoBehaviour
{
    public PuzzleLiquid puzzleLiquid;
    public PuzzleManager puzzleManager;
    public Tube tube;
    public Outline3D outline;
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
        outline.enabled = false;


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
                Debug.Log($"Selected {gameObject.name} as chosen tube");
            }
            else if (puzzleLiquid.chosenTube != null && puzzleLiquid.targetTube == null)
            {
                // Second click: Set as target tube and attempt transfer
                puzzleLiquid.SetTarget(tube);
                puzzleLiquid.TransferLiquid(puzzleLiquid.chosenTube, puzzleLiquid.targetTube);
                ResetSelection();
                puzzleLiquid.SetChosen(null);
                puzzleLiquid.SetTarget(null);
                EvntManager.TriggerEvent("ApplyLitre");
            }
        }

    }

    // Resets the selection state of this button
    private void ResetSelection()
    {
        isChosen = false;
        outline.enabled = false;
        outline.OutlineColor = Color.white;

        // Reset outline for the previously chosen tube
        foreach (var btn in FindObjectsOfType<Btn_Tube>())
        {
            if (btn != this && btn.isChosen)
            {
                btn.isChosen = false;
                btn.outline.enabled = false;
                btn.outline.OutlineColor = Color.white;
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