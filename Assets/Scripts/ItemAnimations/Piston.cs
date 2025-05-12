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
    //eklenince ışık yeşil

    void Start()
    {

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
    
    }
    /*
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isOpen)
        {
            StartCoroutine(StartPistonAnim());
            //taktığında çalışacak animasyon

            indicatorAnimator.SetBool("isOpen", true);
            isOpen = true;
        }

        if (Input.GetKeyDown(KeyCode.E) && isOpen)
        {
            StartCoroutine(StopPistonAnim());
            //çıkardığında çalışacak animator 
            indicatorAnimator.SetBool("isOpen", false);
            isOpen = false;
        }
    }
    */
    public void OnMouseDown()
    {
        Debug.Log("mouse down worked");
        Debug.Log(invSystem.ChosenItem("Steamcore"));
        if (invSystem.ChosenItem("Steamcore") && !isOpen)
        {
            StartCoroutine(StartPistonAnim());
            indicatorAnimator.SetBool("isOpen", true);
            isOpen = true;
            invSystem.RemoveItem("Steamcore", 1);
            embeddedItem.SetActive(true);
            EvntManager.TriggerEvent("subID", 1);
            mat.color = colorGreen;
            mat.SetColor("_EmissionColor", colorGreen);
            return;
        }
        if (isOpen)
        {
            StartCoroutine(StopPistonAnim());
            indicatorAnimator.SetBool("isOpen", false);
            embeddedItem.SetActive(false);
            invSystem.AddItem(ChargedSteamCore);
            isOpen = false;
            mat.color = colorRed;
            mat.SetColor("_EmissionColor", colorRed);

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
}
