using UnityEngine;
using System.Collections;

public class Bomb : MonoBehaviour
{
    [SerializeField] private float moveDuration;
    private bool isMoving = false;
    private Vector3 initialPos;
    private Vector3 lastPos;


    private void Start()
    {
        initialPos = transform.position;
        lastPos = new Vector3(initialPos.x, 0.11f, initialPos.z);
        transform.position = lastPos;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Move(moveDuration));
        }
    }


    private IEnumerator Move(float duration)
    {
        isMoving = true;

        Vector3 start = initialPos;
        Vector3 end = lastPos;

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
