using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Piston : MonoBehaviour
{
    
    [SerializeField] private GameObject indicatorObject;
    public InventorySystem invSystem;
    private Animator indicatorAnimator;
    private Animator pistonAnimator;
    [SerializeField] private float animDuration;

    private float animSpeed = 0f;
    public bool isOpen = false;
    public GameObject embeddedItem;

    public Item ChargedSteamCore;
    public Material mat;
    public Color colorRed;
    public Color colorGreen;
    public GameObject hose;
    public bool canInteract;
    private PuzzleLiquid puzzleLiquid;

    [SerializeField] private float cooldown;
    //eklenince ışık yeşil

    void Start()
    {
        canInteract = true;
        hose.SetActive(false);
        puzzleLiquid = FindAnyObjectByType<PuzzleLiquid>();
        if (ChargedSteamCore == null)
        {
            Debug.Log("ChargedSteamcore is null");
        }
        invSystem = FindAnyObjectByType<InventorySystem>();
        pistonAnimator = GetComponent<Animator>();
        indicatorAnimator = indicatorObject.GetComponent<Animator>();
        // 34. frame = 34 / 30 = 1.133 saniye (30fps animasyon varsay�m�yla)
        pistonAnimator.Play("PistonsOpen", 0, 34f / 30f);
        pistonAnimator.speed = 0f;
        embeddedItem.SetActive(false);
        mat.color = colorRed;
        mat.SetColor("_EmissionColor", colorRed);
        hose.SetActive(false);

    }

    public void OnMouseDown()
    {

        if (invSystem.ChosenItem("SteamcoreCharged") && !isOpen && canInteract)
        {
            StartCoroutine(CanInteract());

            StartCoroutine(StartPistonAnim());

            indicatorAnimator.SetBool("isOpen", true);
            isOpen = true;
            invSystem.RemoveItem("SteamcoreCharged", 1);
            embeddedItem.SetActive(true);
            EvntManager.TriggerEvent("subID", "SCM_INTER");
            mat.color = colorGreen;


            return;
        }
        if (isOpen && canInteract)
        {
            StartCoroutine(StopPistonAnim());
            indicatorAnimator.SetBool("isOpen", false);
            embeddedItem.SetActive(false);
            invSystem.AddItem(ChargedSteamCore);
            isOpen = false;
            mat.color = colorRed;
            mat.SetColor("_EmissionColor", colorRed);

        }
        if (invSystem.ChosenItem("HoseClosed"))
        {
            puzzleLiquid.HoseConnected=true;
            hose.SetActive(true);
            invSystem.RemoveItem("HoseClosed", 1);
        }

    }

    IEnumerator StartPistonAnim()
    {
        float elapsedTime = 0f;
        float startSpeed = 0f;
        float targetSpeed = 3.5f;

        while (elapsedTime < animDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / animDuration);
            animSpeed = Mathf.Lerp(startSpeed, targetSpeed, t); // Yava� yava� h�zlanma
            pistonAnimator.speed = animSpeed;
            yield return null;
        }

        pistonAnimator.speed = targetSpeed; // Sabit 5
        animSpeed = targetSpeed;
    }

    IEnumerator StopPistonAnim()
    {
        float elapsedTime = 0f;
        float startSpeed = animSpeed;
        float targetSpeed = 0f;

        while (elapsedTime < animDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / animDuration);
            animSpeed = Mathf.Lerp(startSpeed, targetSpeed, t); // Yava��a yava�la
            pistonAnimator.speed = animSpeed;
            yield return null;
        }

        pistonAnimator.speed = 0f; // �u anki frame'de dursun
        animSpeed = 0f;
    }
    IEnumerator CanInteract()
    {
        Debug.Log("merhaba");
        canInteract = false;
        yield return new WaitForSeconds(cooldown);
        mat.SetColor("_EmissionColor", colorGreen);
        canInteract = true;
    }
}
