using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Btn_Tube : MonoBehaviour
{
    public PuzzleLiquid puzzleLiquid;
    public PuzzleManager puzzleManager;
    public Tube tube;

    private bool isChosen = false;

    private bool puzzleModeActive;
    void Awake()
    {
        EvntManager.StartListening("GameMode", GameMode);
        EvntManager.StartListening("PuzzleMode", PuzzleMode);
    }

    void Start()
    {

        tube = GetComponentInParent<Tube>();
        puzzleLiquid = FindObjectOfType<PuzzleLiquid>();



    }

   

    public void Move()
    {
        if (puzzleModeActive)
        {
            if (puzzleLiquid.chosenTube == null)
            {
                // First click: Set as chosen tube
                puzzleLiquid.SetChosen(tube);
                isChosen = true;
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


        // Reset outline for the previously chosen tube
        foreach (var btn in FindObjectsOfType<Btn_Tube>())
        {
            if (btn != this && btn.isChosen)
            {
                btn.isChosen = false;
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