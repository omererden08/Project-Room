using UnityEngine;
using DG.Tweening;
public class SteamCoreDragging : MonoBehaviour
{
    public PuzzleManager puzzleManager;

    private bool isDragging;
    private Vector3 offset;
    private float zCoordinate;


    void Start()
    {
        puzzleManager = GetComponentInParent<PuzzleManager>();
    }

    private void OnMouseDown()
    {
        if (!puzzleManager.inPuzzleMode) return;

        zCoordinate = Camera.main.WorldToScreenPoint(transform.position).z;
        offset = transform.position - GetMouseWorldPosition();
        isDragging = true;

        transform.DOKill();
    }

    private void OnMouseDrag()
    {
        if (!puzzleManager.inPuzzleMode) return;

        if (isDragging)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = Camera.main.nearClipPlane;
            transform.position = Camera.main.ScreenToWorldPoint(mousePosition) + offset;
        }
    }

    private void OnMouseUp()
    {
        if (!puzzleManager.inPuzzleMode) return;

        isDragging = false;

    }


    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}