using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using DG.Tweening;

public class TeaLever : IInteractable
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
    public Animator lightAnim;
    public bool TeaClock;

    private LayerMask interactLayer;
    private bool isRotating = false;
    [SerializeField] private bool isMoving = false;
    public bool isPulling = false;
    public bool canLeverPull = true;
    public bool inSlot = false;
    public GameObject token;
    public Vector3 sPos;
    public TeaCheck teaCheck;
    public TeaAnimation teaAnimation;
    private void Start()
    {
        outline = GetComponent<Outline3D>();
        outline.enabled = false;
        sPos = teaCup.transform.position;
        leverAnim = GetComponent<Animator>();
        indicatorAnim = teaIndicator.GetComponent<Animator>();
        rightDoorAnim = rightDoor.GetComponent<Animator>();
        leftDoorAnim = leftDoor.GetComponent<Animator>();
        teaCheck = FindAnyObjectByType<TeaCheck>();
        teaAnimation = FindAnyObjectByType<TeaAnimation>();
        interactLayer = 1 << gameObject.layer; // Sadece kendi layer'ı için, gerekirse kaldır
        EvntManager.StartListening<bool>("SetCanLeverPull", SetCanLeverPull);
        EvntManager.StartListening("MoveCupToStart", MoveCupToStart);
        inSlot = false;
    }

    public override void Interact()
    {
        Debug.Log($"Interact çağrıldı. isRotating: {isRotating}, canLeverPull: {canLeverPull}, TeaClock: {TeaClock}");
        if (!isRotating && canLeverPull)
        {
            StartCoroutine(WorkingMachine());

            if (TeaClock)
            {
                token.SetActive(true);  //bug
            }
        }
        else if (!isRotating && !canLeverPull)
        {
            StartCoroutine(TryPulling());
        }
    }

    public void StartCupMove()
    {
        if (!isMoving)
            StartCoroutine(MoveCupToTarget());
    }
    public void SetCanLeverPull(bool value)
    {
        canLeverPull = value;
    }

    private IEnumerator PullingLever(bool down)
    {
        isRotating = true;
        lightAnim.SetTrigger("Play");
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
        //yerrelde tut
        Debug.Log("abugat hareket ediyor");
        Vector3 startPos = teaCup.transform.position;
        Vector3 targetPos = cupTarget.position;

        teaCup.transform.DOMove(targetPos, fallDuration).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            //bardak burada düşüyor ve kontrol edilecek
            canLeverPull = false;
            inSlot = true;
            Debug.Log("abugat düştü");
            isMoving = false;
        });

        yield return null;
        isMoving = false;
    }
    private void MoveCupToStart()
    {
        teaCup.transform.position = sPos;
        teaCup.SetActive(true);
        inSlot = false;
        canLeverPull = true; // Lever tekrar çekilebilir hale gelir
        Debug.Log("TeaCup başlangıç konumuna döndü. canLeverPull: " + canLeverPull);
        teaAnimation.canDrinkable = true;
        teaCheck.isDrinked = false;
        teaAnimation.Fill();
        EvntManager.TriggerEvent("SetTea"); // SetTea olayını tetikle
    }
}
