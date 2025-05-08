using System.Collections;
using UnityEngine;

public class TeaLever : MonoBehaviour
{
    [SerializeField] private GameObject teaIndicator;
    [SerializeField] private GameObject rightDoor;
    [SerializeField] private GameObject leftDoor;
    [SerializeField] private GameObject teaCup;
    [SerializeField] private Transform cupTarget;
    [SerializeField] private float animationDuration;
    [SerializeField] private float moveDuration;
    [SerializeField] private float fallDuration;
    private Animator leverAnim;
    private Animator indicatorAnim;
    private Animator rightDoorAnim;
    private Animator leftDoorAnim;

    private LayerMask interactLayer;
    private bool isRotating = false;
    private bool isMoving = false;
    public bool isPulling = false; 
    public bool canLeverPull = true;

    private void Start()
    {
        leverAnim = GetComponent<Animator>();
        indicatorAnim = teaIndicator.GetComponent<Animator>();
        rightDoorAnim = rightDoor.GetComponent<Animator>();
        leftDoorAnim = leftDoor.GetComponent<Animator>();
        interactLayer = 1 << gameObject.layer; // Sadece kendi layer'ı için, gerekirse kaldır
    }

    private void Update()
    {
        //teaCup.transform.position = cupTarget.position;

        if (Input.GetMouseButtonDown(0))
        {
            TryRotate();
        }

    }

    void TryRotate()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 3f, interactLayer))
        {
            if (hit.transform == transform && !isRotating && canLeverPull)
            {
                StartCoroutine(WorkingMachine());
            }
            else if (hit.transform == transform && !isRotating && !canLeverPull)
            {
                StartCoroutine(TryPulling());
            }
        }
    }


    void StartCupMove()
    {
        if (!isMoving)
            StartCoroutine(MoveCupToTarget());
    }


    private IEnumerator PullingLever(bool down)
    {
        isRotating = true;

        Quaternion startRot = transform.localRotation;
        Quaternion endRot = Quaternion.Euler(down ? -113f : -68f, -63f, 90f);

        float elapsed = 0f;
        while (elapsed < animationDuration)
        {
            float t = elapsed / animationDuration;
            transform.localRotation = Quaternion.Slerp(startRot, endRot, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = endRot;
        isRotating = false;
    }

    private IEnumerator TryPulling()
    {
        isRotating = true;

        Quaternion startRot = transform.localRotation;
        Quaternion endRot = Quaternion.Euler(-108f, -63f, 90f);

        float elapsed = 0f;
        while (elapsed < 0.3f)
        {
            float t = elapsed / 0.3f;
            transform.localRotation = Quaternion.Slerp(startRot, endRot, t);
           
            elapsed += Time.deltaTime;
            yield return null;
        }

        while (elapsed < 0.3f)
        {
            float t = elapsed / 0.3f;
            transform.localRotation = Quaternion.Slerp(endRot, startRot, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = startRot;
        isRotating = false;
    }


    private IEnumerator IndicatorRoutine(bool isActive)
    {
        indicatorAnim.SetBool("isWorking", isActive);

        yield return null;
    }

    private IEnumerator WorkingMachine()
    {
        isRotating = true;
        yield return StartCoroutine(PullingLever(false));
        yield return StartCoroutine(IndicatorRoutine(true));

        isPulling = true;

        yield return new WaitForSeconds(moveDuration);

        rightDoorAnim.SetTrigger("Open");
        leftDoorAnim.SetTrigger("Open");
        yield return new WaitForSeconds(0.6f);

        StartCupMove();

        yield return new WaitForSeconds(moveDuration);

        yield return StartCoroutine(PullingLever(true));
        yield return StartCoroutine(IndicatorRoutine(false));

        isPulling = false;

        isRotating = false;
    }


    private IEnumerator MoveCupToTarget()
    {
        isMoving = true;

        Vector3 startPos = teaCup.transform.position;
        Vector3 targetPos = cupTarget.position;

        float elapsed = 0f;

        while (elapsed < fallDuration)
        {
            float t = elapsed / fallDuration;
            teaCup.transform.position = Vector3.Lerp(startPos, targetPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        teaCup.transform.position = targetPos;
        isMoving = false;
    }



}
