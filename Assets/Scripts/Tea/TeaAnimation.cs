using UnityEditorInternal;
using UnityEngine;
using DG.Tweening;
using System.Collections;
using Unity.Mathematics;
public class TeaAnimation : MonoBehaviour
{
    public GameObject top;
    public float drinkDuration;
    public bool canDrinkable;
    public Transform endPoint;
    public GameObject Cup;
    public GameObject Cam;
    public TeaSlot teaSlot;
    public Item item;
    void Start()
    {
        teaSlot = FindAnyObjectByType<TeaSlot>();
        EvntManager.StartListening("DrinkTea", Drink);
        Cam = Camera.main.gameObject;
        canDrinkable = true;
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
        top.transform.DOScale(new Vector3(0.6f, 0.6f, 0.6f), drinkDuration);

        top.transform.DOMove(endPoint.position, drinkDuration).onComplete += () =>
        {
            InventorySystem.Instance.AddItem(item);
            teaSlot.isFilled = false;
            teaSlot.CheckState();
            Cup.SetActive(false);
            top.SetActive(false);
        };;
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
