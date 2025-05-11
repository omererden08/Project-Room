using UnityEditorInternal;
using UnityEngine;
using DG.Tweening;
using System.Collections;
using Unity.Mathematics;
using UnityEngine.UI;
public class TeaAnimation : MonoBehaviour
{
    public GameObject top;
    public Vector3 topPos;
    public float drinkDuration;
    public bool canDrinkable;
    public Transform endPoint;
    public GameObject Cup;
    public GameObject Cam;
    public TeaSlot teaSlot;
    public Item item;
    public Vector3 startPoint;
    public Vector3 startSize;
    public TeaCheck teaCheck;
    public TeaAnimation teaAnimation;
    void Start()
    {
        teaCheck = FindAnyObjectByType<TeaCheck>();
        teaSlot = FindAnyObjectByType<TeaSlot>();
        teaAnimation = FindAnyObjectByType<TeaAnimation>();
        EvntManager.StartListening("DrinkTea", Drink);
        topPos = top.transform.position;
        Cam = Camera.main.gameObject;
        canDrinkable = true;
        startPoint = transform.position;
        teaSlot.spawnPoint = startPoint;
        StartCoroutine(ChockMovement(0.2f));
    }

    public void Wobble()
    {
        top.transform.DOShakeRotation(2f, 40f, 5);
    }

    public void Drink()
    {
        if(!canDrinkable)
        {
            return;
        }
        top.transform.DOScale(new Vector3(0.04f, 0.04f, 0.04f), drinkDuration);

        top.transform.DOMove(endPoint.position, drinkDuration).onComplete += () =>
        {
            InventorySystem.Instance.AddItem(item);
            teaSlot.isFilled = false;
            teaSlot.CheckState();
            Cup.SetActive(false);
            top.SetActive(false);
        };;
    }
    public void Fill()
    {
        top.transform.DOScale(startSize, drinkDuration);
        top.transform.position = topPos;
        top.SetActive(true);
        canDrinkable = true;
        Debug.Log("TeaAnimation dolduruldu. canDrinkable: " + canDrinkable);
    }
    public void TokenMod()
    {
        top.SetActive(false);
        canDrinkable = false;
    }
    public IEnumerator ChockMovement(float seconds)
    {
        while (true)
        {
            Vector3 nowPos = transform.position;
            Vector3 nowRot = transform.rotation.eulerAngles;
            yield return new WaitForSeconds(seconds);

            Vector3 newPos = transform.position;
            Vector3 newRot = transform.rotation.eulerAngles;

            float distanceMoved = Vector3.Distance(nowPos, newPos);
            float rotationChanged = Vector3.Distance(nowRot, newRot);

            if (distanceMoved > 0.4f || rotationChanged > 0.4f)
            {
                Debug.Log("Wobble");
                Wobble();
            }
        }

    }
}
