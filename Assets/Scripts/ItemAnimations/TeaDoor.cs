using UnityEngine;
using System.Collections;

public class TeaDoor : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private float animationDuration = 1f;

    private TeaLever teaLever;
    private bool isMoving = false;
    private bool isOpen = false;

    private void Start()
    {
        // Bu atama gereksiz olabilir; animator kullanýlmýyor
        if (animator == null)
            animator = GetComponent<Animator>();

        if (teaLever == null)
            teaLever = FindObjectOfType<TeaLever>();

        // interactLayer baþlangýçta ayarlanmadýysa, elle ayarlanmasý daha saðlýklý olur
        if (interactLayer == 0)
            interactLayer = 1 << gameObject.layer;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ToggleDoor();
        }
    }

    private void ToggleDoor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 3f, interactLayer))
        {
            if (hit.transform == transform && !isMoving)
            {
                if (!isOpen && !teaLever.isPulling)
                {
                    StartCoroutine(DoorMove(true));
                    isOpen = true;
                    teaLever.canLeverPull = false;
                }
                else if (isOpen)
                {
                    StartCoroutine(DoorMove(false));
                    isOpen = false;
                    teaLever.canLeverPull = true;
                }
            }
        }
    }

    private IEnumerator DoorMove(bool open)
    {
        isMoving = true;

        Quaternion startRot = transform.localRotation;
        Quaternion endRot = Quaternion.Euler(-90f, 0f, open ? 135f : 0f);

        float elapsed = 0f;
        while (elapsed < animationDuration)
        {
            float t = elapsed / animationDuration;

            transform.localRotation = Quaternion.Slerp(startRot, endRot, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = endRot;
        isMoving = false;
    }
}
