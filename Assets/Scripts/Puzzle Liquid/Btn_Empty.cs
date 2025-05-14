using UnityEngine;

public class Btn_Empty : MonoBehaviour
{
    private Tube tube;
    private PuzzleLiquid puzzleLiquid;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tube = GetComponentInParent<Tube>();
        puzzleLiquid = FindObjectOfType<PuzzleLiquid>();
    }


    /*
    void OnMouseDown()
    {
        puzzleLiquid.EmptyTube(tube);
    }
    */
    public void Empty()
    {
        puzzleLiquid.EmptyTube(tube);
    }
}
