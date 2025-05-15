using UnityEngine;
using System.Collections;

public class SafeRotate : IInteractable
{
    [SerializeField] private Transform doorPivot;
    [SerializeField] private float duration = 1f;
    [SerializeField] private float openAngle = -90f;
    [SerializeField] private float rotateDuration;

    private Quaternion closedRot;
    private Quaternion openRot;
    private bool isLocked = true;
    private bool isMoving = false;
    private bool isOpened = false;

    private Animator safeKnobAnim;
    [SerializeField] private Animator openAnim;

    //public AudioClip Unlocked;



    public void Start()
    {

        bomb = FindAnyObjectByType<Bomb>();
        EvntManager.StartListening("SafeOpen", UnlockedSafe);

        closedRot = doorPivot.rotation;
        openRot = closedRot * Quaternion.Euler(0, openAngle, 0);

        safeKnobAnim = GetComponent<Animator>();
    }



    public override void Interact()
    {
        if (isLocked)
            SafeKnobRotate();
        else
            StartCoroutine(ToggleDoor());
    }

    void UnlockedSafe()
    {
        isLocked = false;
    }

    void SafeKnobRotate()
    {
        if (!isOpened)
        {
            if (isLocked)
            {
                safeKnobAnim.SetTrigger("Wrong");
            }
            else
            {
                //AudioManager.Instance.audioSource.PlayOneShot(Unlocked);
                safeKnobAnim.SetTrigger("Correct");
            }
        }
        else
        {
            safeKnobAnim.SetTrigger("Wrong");
        }
    }

    public IEnumerator ToggleDoor()
    {
        SafeKnobRotate();

        yield return new WaitForSeconds(rotateDuration); // Animasyon s�resi kadar bekle

        if (!isMoving && !isLocked)
        {
            OpenDoor();
            isOpened = true;
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
