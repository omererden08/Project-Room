using UnityEngine;
using System.Collections;

public class SteamcoreValve : IInteractable
{
    [Header("Indicator")]
    [SerializeField] private GameObject indicator;
    [SerializeField] private string indicatorAnimationName;
    [SerializeField] private float animationSpeed = 1f;

    public float currentFrame = 0f; // Mevcut frame
    private float targetFrame = 0f;  // Hedef frame
    private Animator indicatorAnimator;

    [Header("Valve")]
    [SerializeField] private float rotateDuration = 1f;
    [SerializeField] private float rotationAmount = 180f;

    private bool isRotating = false;
    private Quaternion targetRotation;

    public bool canSpin;
    private void Start()
    {
        targetRotation = transform.rotation;
        outline = GetComponent<Outline3D>();
        outline.enabled = false;
        indicatorAnimator = indicator.GetComponent<Animator>();

        SetIndicatorFrame(currentFrame);
        canSpin = false;
        EvntManager.StartListening("CanSpinValves", CanSpinValves);
    }

    public void CanSpinValves()
    {
        canSpin = true;
    }
    public override void Interact()
    {
        if (isRotating) return;
        if (!canSpin) return;

        // Frame hedefini güncelle (her seferinde 5 ilerlet, 20 sonrası sıfırla)
        targetFrame += 5f;
        if (targetFrame > 20f) targetFrame = 0f;

        targetRotation *= Quaternion.Euler(-rotationAmount, 0f, 0f);

        StartCoroutine(RotateValve());
    }

    private IEnumerator RotateValve()
    {
        isRotating = true;

        Quaternion startRot = transform.rotation;
        Quaternion endRot = targetRotation;

        float elapsedTime = 0f;

        float startFrame = currentFrame;
        float endFrame = targetFrame;

        indicatorAnimator.Play(indicatorAnimationName, 0, startFrame / 30f);
        indicatorAnimator.speed = animationSpeed;

        while (elapsedTime < rotateDuration)
        {
            float t = elapsedTime / rotateDuration;

            // Valve dönüşü
            transform.rotation = Quaternion.Slerp(startRot, endRot, t);

            // Frame geçişi (yumuşak)
            currentFrame = Mathf.Lerp(startFrame, endFrame, t);
            float normalizedTime = currentFrame / 30f;

            indicatorAnimator.Play(indicatorAnimationName, 0, normalizedTime);

            // 🔍 Frame debug

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Debug.Log($"{gameObject.name} currentFrame: {Mathf.RoundToInt(currentFrame)}");

        transform.rotation = endRot;
        currentFrame = targetFrame;
        SetIndicatorFrame(currentFrame);

        isRotating = false;
    }




    private void SetIndicatorFrame(float frame)
    {
        float normalizedTime = frame / 30f;
        indicatorAnimator.Play(indicatorAnimationName, 0, normalizedTime);
        indicatorAnimator.speed = 0f;
    }
}
