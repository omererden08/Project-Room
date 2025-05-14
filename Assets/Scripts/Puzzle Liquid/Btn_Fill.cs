using UnityEngine;

public class Btn_Fill : MonoBehaviour
{
    private Tube tube;
    private PuzzleLiquid puzzleLiquid;
    private ButtonMover buttonMover;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tube = GetComponentInParent<Tube>();
        puzzleLiquid = FindObjectOfType<PuzzleLiquid>();
        buttonMover = GetComponent<ButtonMover>();
    }


    void OnMouseDown()
    {
        puzzleLiquid.FillTube(tube);
    }

    void Fill()
    {
        
    }
}

