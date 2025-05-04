using UnityEngine;

public class Disk : MonoBehaviour
{
    public float spinSpeed = 90f; // Degrees per second (positive = clockwise, negative = counterclockwise)
    private float currentSpinDirection = 0f; // Current spin direction (1, -1, or 0)
    private bool isSpinning = false;
    [Tooltip("Disk numarası, girilmesi gerekli")]public int DiskNo;

    [Tooltip("saat yönü 1, saat tersi -1")] public int rotation = 1;

    void Update()
    {
        if (isSpinning && currentSpinDirection != 0)
        {
            // Rotate smoothly around Y-axis
            float rotationAmount = currentSpinDirection * spinSpeed * Time.deltaTime;
            transform.Rotate(0, rotationAmount, 0, Space.Self);
        }
    }

    public void Spin(int direction)
    {
        if (direction == 0)
        {
            Debug.Log($"Disk {name} received spin direction 0, stopping rotation.");
            Stop();
            return;
        }

        currentSpinDirection = direction; // 1 for clockwise, -1 for counterclockwise
        isSpinning = true;
        Debug.Log($"Disk {name} started smooth spinning with direction {direction} ({(direction > 0 ? "clockwise" : "counterclockwise")}) at {spinSpeed} deg/s.");
    }

    public void Stop()
    {
        isSpinning = false;
        currentSpinDirection = 0f;
        Debug.Log($"Disk {name} stopped spinning.");
    }

    public void SetRotation(int rotation)
    {
        this.rotation = rotation;
        Debug.Log($"Disk {name} rotation direction set to {rotation}");
    }
}