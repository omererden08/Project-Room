using System.Collections;
using UnityEngine;

public class DrawerAnimation : IInteractable
{
    [Header("Movement")]
    [SerializeField] private Transform targetPos;
    [SerializeField] private GameObject keyObject;
    private Animator keyAnimator;
    private float moveDuration = 0.55f;

    [Header("Interaction")]
    [SerializeField] private LayerMask interactLayer;

    [Header("Sounds")]
    public AudioClip Locked;
    public AudioClip Opened;
    public AudioClip Unlocked;
    private Vector3 initialPos;
    [SerializeField] private bool isMoving = false;
    private bool isOpen = false;
    private bool isLocked = true;  //select object key olunca false olacak 

    public enum DrawerType { Normal, Locked }

    [SerializeField] private DrawerType drawerType = DrawerType.Normal;



    void Start()
    {
        outline = GetComponent<Outline3D>();
        initialPos = transform.position;
        if (drawerType == DrawerType.Locked)
        {
            keyAnimator = keyObject.GetComponent<Animator>();
        }


        if (targetPos == null && transform.childCount > 0)
        {
            targetPos = transform.GetChild(0);
        }

        if (interactLayer.value == 0)
        {
            Debug.LogWarning($"{name}: InteractLayer is not set in the inspector.");
        }
    }

    public override void Interact()
    {
        if (isMoving) return;

        if (drawerType == DrawerType.Locked)
        {
            if (isLocked)
            {
                AudioManager.Instance.audioSource.PlayOneShot(Locked);
                Debug.Log("�ekmece kilitli, anahtar gerekiyor!");
                return; // Kilitliyse ve a��lmam��sa hi�bir �ey yapma
            }
            else
            {
                isMoving = true;
                StartCoroutine(OpenLockedSequence());
                AudioManager.Instance.audioSource.PlayOneShot(Unlocked);
                drawerType = DrawerType.Normal; // Kilit a��ld�ktan sonra normal �ekmeceye ge�
            }
        }
        else
        {
            isMoving = true;
            AudioManager.Instance.audioSource.PlayOneShot(Opened);
            ToggleDrawer(moveDuration); // normal �ekmece

        }
    }


    private IEnumerator OpenLockedSequence()
    {

        if (keyObject != null)
            keyObject.SetActive(true);

        if (keyAnimator != null)
            keyAnimator.SetTrigger("Open");

        yield return new WaitForSeconds(1f);

        ToggleDrawer(moveDuration);

    }

    void ToggleDrawer(float duration)
    {
        isOpen = !isOpen;
        StartCoroutine(Move(isOpen, duration));
    }


    private IEnumerator Move(bool open, float duration)
    {
        Vector3 start = transform.position;
        Vector3 end = open ? targetPos.position : initialPos;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.position = Vector3.Lerp(start, end, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = end;
        isMoving = false;
    }


}
