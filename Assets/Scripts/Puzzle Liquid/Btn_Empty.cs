using UnityEngine;

public class Btn_Empty : MonoBehaviour
{
    public Tube tube;
    public PuzzleLiquid puzzleLiquid;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tube = GetComponentInParent<Tube>();
        puzzleLiquid = FindObjectOfType<PuzzleLiquid>();
    }



    void OnMouseDown()
    {
        puzzleLiquid.EmptyTube(tube);
    }
}
