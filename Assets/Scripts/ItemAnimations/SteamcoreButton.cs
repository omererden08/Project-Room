using UnityEngine;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime;
using DG.Tweening;
public class SteamcoreButton : IInteractable
{
    [Header("IndicatorObjects")]
    [SerializeField] private GameObject steamcoreObject;
    [SerializeField] private GameObject firstIndicatorObject;
    [SerializeField] private GameObject secondIndicatorObject;

    private Animator steamcoreAnimator;
    private Animator firstIndicatorAnimator;
    private Animator secondIndicatorAnimator;

    [Header("PuzzleStatements")]
    [SerializeField] private GameObject[] puzzleObjects;  // atili objeler icindeki currentframe degerleri sifre olacak belirledigim degerlerde checkpassword yapacak dogruysa sifre dogru yazacak
    [SerializeField] private float[] correctCombination; // örneğin [5, 10, 0]


    private bool isPuzzleCompleted = false;
    [Header("ButtonMovement")]
    [SerializeField] private Transform target;
    [SerializeField] private float moveDuration;
    private Vector3 initialPos;
    private bool isMoving = false;
    private bool isOpen = false;
    public CollectableItem collectableItem;
    public Material mat;
    private void Start()
    {
        target = transform.GetChild(0);
        steamcoreAnimator = steamcoreObject.GetComponent<Animator>();
        firstIndicatorAnimator = firstIndicatorObject.GetComponent<Animator>();
        secondIndicatorAnimator = secondIndicatorObject.GetComponent<Animator>();
        mat.SetColor("_EmissionColor", Color.red);
        mat.SetColor("_Color", Color.red);
        collectableItem.enabled = false;

    }

    public override void Interact()
    {
        if (isMoving) return;

        isMoving = true;

        ToggleButton();

        // Şifre kontrolü
        if (!isPuzzleCompleted)
            isPuzzleCompleted = CheckPassword(); // Artık durum doğru atanıyor


        if (isPuzzleCompleted)
        {
            //material green
            mat.SetColor("_EmissionColor", Color.green); // Green emision color set
            mat.SetColor("_Color", Color.green); // Green color set

            steamcoreAnimator.SetTrigger("Open");

            if (steamcoreAnimator != null)
            {
                firstIndicatorAnimator.SetTrigger("Open");
                secondIndicatorAnimator.SetTrigger("Open");
                collectableItem.enabled = true;
            }

            steamcoreAnimator = null;
        }
       
        
        //isik yanacak
        //ses eklenecek 


        
    }


    void ToggleButton()
    {
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        Vector3 start = transform.position;
        Vector3 end = target.position;
        float elapsedTime = 0f;

        isMoving = true;

        // Gidiş (start → end)
        while (elapsedTime < moveDuration)
        {
            float t = elapsedTime / moveDuration;
            transform.position = Vector3.Lerp(start, end, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = end; // Tam hizala
        elapsedTime = 0f;

        // Dönüş (end → start)
        while (elapsedTime < moveDuration)
        {
            float t = elapsedTime / moveDuration;
            transform.position = Vector3.Lerp(end, start, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = start; // Tam hizala
        isMoving = false;
    }

    private bool CheckPassword()
    {
        if (puzzleObjects.Length != correctCombination.Length)
        {
            Debug.LogError("Puzzle config uyuşmuyor.");
            return false;
        }

        for (int i = 0; i < puzzleObjects.Length; i++)
        {
            SteamcoreValve valve = puzzleObjects[i].GetComponent<SteamcoreValve>();
            if (valve == null)
            {
                Debug.LogError($"Puzzle object {i} SteamcoreValve component eksik!");
                return false;
            }

            if (Mathf.RoundToInt(valve.currentFrame) != Mathf.RoundToInt(correctCombination[i]))
            {
                Debug.Log("Şifre yanlış.");
                return false;
            }
        }

        Debug.Log("Şifre doğru!");
        return true;
    }



}



