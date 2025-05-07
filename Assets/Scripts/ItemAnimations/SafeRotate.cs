using UnityEngine;
using System.Collections;

public class SafeRotate : MonoBehaviour
{
    [SerializeField] private Transform doorPivot;
    [SerializeField] private float duration = 1f;
    [SerializeField] private float openAngle = -90f;
    [SerializeField] private float rotateDuration;

    private Quaternion closedRot;
    private Quaternion openRot;
    private bool isLocked = true;
    private bool isMoving = false;
    private LayerMask interactLayer;

    private Transform safeKnob;
    private Animator safeKnobAnim;
    private Animator openAnim;

    void Start()
    {
        EvntManager.StartListening("SafeOpen", UnlockedSafe);

        closedRot = doorPivot.rotation;
        openRot = closedRot * Quaternion.Euler(0, openAngle, 0);

        // Referanslarý önceden cache’le
        safeKnob = transform.GetChild(1);
        safeKnobAnim = safeKnob.GetComponent<Animator>();
        openAnim = GetComponentInParent<Animator>();

        // Layer mask olarak ayarla
        interactLayer = 1 << safeKnob.gameObject.layer; // Layer index -> bitmask
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isLocked)
                SafeKnobRotate();
            else
                StartCoroutine(ToggleDoor());
        }
    }

    void UnlockedSafe()
    {
        isLocked = false;
    }

    void SafeKnobRotate()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 3f, interactLayer))
        {
            if (hit.transform != safeKnob) return; // Sadece safeKnob objesine týklanýrsa

            if (isLocked)
            {
                safeKnobAnim.SetTrigger("Wrong");
            }
            else
            {
                safeKnobAnim.SetTrigger("Correct");
            }
        }
    }

    public IEnumerator ToggleDoor()
    {
        SafeKnobRotate();

        yield return new WaitForSeconds(rotateDuration); // Animasyon süresi kadar bekle

        if (!isMoving && !isLocked)
        {
            OpenDoor();
        }
    }

    void OpenDoor()
    {
        if (openAnim != null)
        {
            openAnim.SetTrigger("Open");
        }
    }
}
