using UnityEngine;
using System.Collections;

public class Bomb : MonoBehaviour
{
    public bool isBoombReady;
    [SerializeField] private float moveDuration;
    [SerializeField] private Collider colliderBomb;
    private bool isMoving = false;
    private Vector3 initialPos;
    private Vector3 lastPos;
    private float y = -0.008085489f;


    private void Start()
    {
        colliderBomb.enabled = false;
        isBoombReady = false;
        initialPos = transform.position;
        lastPos = new Vector3(initialPos.x, y, initialPos.z);
        EvntManager.StartListening("BombUp", BombUp);
    }

    private void BombUp()
    {
        colliderBomb.enabled = true;

        StartCoroutine(Move(moveDuration));
    }
    private IEnumerator Move(float duration)
    {
        isMoving = true;
        colliderBomb.enabled = true;
        Vector3 start = initialPos;
        Vector3 end = lastPos;

        float elapsedTime = 0f;
        EvntManager.TriggerEvent("CameraShakeBig");
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.position = Vector3.Lerp(start, end, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        EvntManager.TriggerEvent("startTimer");
        isBoombReady = true;
        colliderBomb.enabled = false;
        transform.position = end;
        isMoving = false;
    }
}
