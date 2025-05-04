using UnityEngine;
using System.Collections;

public class DevreSystem : MonoBehaviour
{
    public Disk[] disks;

    public Disk disk1;
    public Disk disk2;
    public Disk disk3;

    public CoreSlotDisk[] coreSlotDisks;

    [Tooltip("Initial direction: 1 for clockwise, -1 for counterclockwise")]
    public int disk1Direction;
    [Tooltip("Initial direction: 1 for clockwise, -1 for counterclockwise")]
    public int disk2Direction;
    [Tooltip("Initial direction: 1 for clockwise, -1 for counterclockwise")]
    public int disk3Direction;

    [Tooltip("Target Y rotation for disk1 (degrees)")]
    public float disk1TargetAngle = 0f;
    [Tooltip("Target Y rotation for disk2 (degrees)")]
    public float disk2TargetAngle = 0f;
    [Tooltip("Target Y rotation for disk3 (degrees)")]
    public float disk3TargetAngle = 0f;
    [Tooltip("Tolerance for win condition (degrees)")]
    public float toleranceDegrees = 5f;

    private bool isCheckingWin = false;
    private bool hasWon = false;
    private int[] lastSpinDirections = new int[3]; // Cache spin directions for disk1, disk2, disk3

    public void CalculateSpin(CoreSlotDisk triggeringSlot)
    {
        Debug.Log($"CalculateSpin called for slot {triggeringSlot.name} with stat: {triggeringSlot.stat}, isActive: {triggeringSlot.isActive()}");

        bool hasL = false;
        bool hasM = false;
        bool hasR = false;

        // Check all slots in coreSlotDisks
        Debug.Log($"Checking {coreSlotDisks.Length} slots in coreSlotDisks array");
        foreach (var slot in coreSlotDisks)
        {
            if (slot.isActive())
            {
                Debug.Log($"Active slot found: {slot.name} with stat: {slot.stat}");
                switch (slot.stat)
                {
                    case CoreSlotStat.L:
                        hasL = true;
                        break;
                    case CoreSlotStat.M:
                        hasM = true;
                        break;
                    case CoreSlotStat.R:
                        hasR = true;
                        break;
                }
            }
        }

        Debug.Log($"Slot states - L: {hasL}, M: {hasM}, R: {hasR}");

        // Calculate new spin directions
        int disk1Dir = 0, disk2Dir = 0, disk3Dir = 0;
        if (hasL && !hasM && !hasR)
        {
            Debug.Log("L only: Spinning disk1 (+1), disk2 (+1)");
            disk1Dir = 1;
            disk2Dir = 1;
        }
        else if (!hasL && hasM && !hasR)
        {
            Debug.Log("M only: Spinning disk1 (-1), disk2 (-1), disk3 (-1)");
            disk1Dir = -1;
            disk2Dir = -1;
            disk3Dir = -1;
        }
        else if (!hasL && !hasM && hasR)
        {
            Debug.Log("R only: Spinning disk2 (+1), disk3 (+1)");
            disk2Dir = 1;
            disk3Dir = 1;
        }
        else if (hasL && hasM && !hasR)
        {
            Debug.Log("L + M: Spinning disk1 (+1), disk2 (-1), disk3 (-1)");
            disk1Dir = 1;
            disk2Dir = -1;
            disk3Dir = -1;
        }
        else if (!hasL && hasM && hasR)
        {
            Debug.Log("M + R: Spinning disk1 (-1), disk2 (-1), disk3 (+1)");
            disk1Dir = -1;
            disk2Dir = -1;
            disk3Dir = 1;
        }
        else if (hasL && !hasM && hasR)
        {
            Debug.Log("L + R: Spinning disk1 (+1), disk2 (+1), disk3 (+1)");
            disk1Dir = 1;
            disk2Dir = 1;
            disk3Dir = 1;
        }
        else if (hasL && hasM && hasR)
        {
            Debug.Log("L + M + R: Spinning disk1 (0), disk2 (0), disk3 (0)");
            disk1Dir = 0;
            disk2Dir = 0;
            disk3Dir = 0;
        }
        else
        {
            Debug.Log("No slots active: No spinning");
        }

        // Apply spins only if directions changed
        if (disk1Dir != lastSpinDirections[0])
        {
            disk1.Stop();
            disk1.Spin(disk1Dir);
            lastSpinDirections[0] = disk1Dir;
        }
        if (disk2Dir != lastSpinDirections[1])
        {
            disk2.Stop();
            disk2.Spin(disk2Dir);
            lastSpinDirections[1] = disk2Dir;
        }
        if (disk3Dir != lastSpinDirections[2])
        {
            disk3.Stop();
            disk3.Spin(disk3Dir);
            lastSpinDirections[2] = disk3Dir;
        }
    }

    public void NotifySlotStateChanged()
    {
        bool anySlotActive = false;
        foreach (var slot in coreSlotDisks)
        {
            if (slot.isActive())
            {
                anySlotActive = true;
                break;
            }
        }

        if (anySlotActive && !isCheckingWin && !hasWon)
        {
            Debug.Log("Starting win condition check coroutine");
            StartCoroutine(CheckWinConditionCoroutine());
        }
        else if (!anySlotActive && isCheckingWin)
        {
            Debug.Log("Stopping win condition check coroutine");
            StopCoroutine(CheckWinConditionCoroutine());
            isCheckingWin = false;
        }
    }

    private IEnumerator CheckWinConditionCoroutine()
    {
        isCheckingWin = true;
        while (isCheckingWin && !hasWon)
        {
            CheckWinCondition();
            yield return new WaitForSeconds(1f); // Check every 1 second
        }
    }

    private void CheckWinCondition()
    {
        if (hasWon) return;

        float disk1Angle = NormalizeAngle(disk1.transform.eulerAngles.y);
        float disk2Angle = NormalizeAngle(disk2.transform.eulerAngles.y);
        float disk3Angle = NormalizeAngle(disk3.transform.eulerAngles.y);

        float disk1Target = NormalizeAngle(disk1TargetAngle);
        float disk2Target = NormalizeAngle(disk2TargetAngle);
        float disk3Target = NormalizeAngle(disk3TargetAngle);

        bool disk1Correct = Mathf.Abs(disk1Angle - disk1Target) <= toleranceDegrees;
        bool disk2Correct = Mathf.Abs(disk2Angle - disk2Target) <= toleranceDegrees;
        bool disk3Correct = Mathf.Abs(disk3Angle - disk3Target) <= toleranceDegrees;

        Debug.Log($"Win check: Disk1={disk1Angle:F1}° (Target={disk1Target:F1}°, Correct={disk1Correct}), " +
                  $"Disk2={disk2Angle:F1}° (Target={disk2Target:F1}°, Correct={disk2Correct}), " +
                  $"Disk3={disk3Angle:F1}° (Target={disk3Target:F1}°, Correct={disk3Correct})");

        if (disk1Correct && disk2Correct && disk3Correct)
        {
            Debug.Log("Puzzle Complete! All disks are within tolerance of target angles.");
            hasWon = true;
            isCheckingWin = false;
            disk1.Stop();
            disk2.Stop();
            disk3.Stop();
        }
    }

    private float NormalizeAngle(float angle)
    {
        angle = angle % 360;
        if (angle < 0) angle += 360;
        return angle;
    }
    void Awake()
    {
        disks = FindObjectsByType<Disk>(FindObjectsSortMode.None);
        foreach (var disk in disks)
        {
            switch (disk.DiskNo)
            {
                case 1:
                    disk1 = disk;
                    break;
                case 2:
                    disk2 = disk;
                    break;
                case 3:
                    disk3 = disk;
                    break;
            }
        }
    }
    public void Start()
    {

        if (disk1 == null || disk2 == null || disk3 == null)
        {
            Debug.LogError("One or more disks are not assigned in DevreSystem!", this);
        }
        if (coreSlotDisks == null || coreSlotDisks.Length == 0)
        {
            Debug.LogWarning("coreSlotDisks array is empty or null! Attempting to find CoreSlotDisk components in scene.", this);
            coreSlotDisks = FindObjectsOfType<CoreSlotDisk>();
            if (coreSlotDisks.Length == 0)
            {
                Debug.LogError("No CoreSlotDisk components found in scene!", this);
            }
            else
            {
                Debug.Log($"Found {coreSlotDisks.Length} CoreSlotDisks: {string.Join(", ", System.Linq.Enumerable.Select(coreSlotDisks, s => s.name))}");
            }
        }
        else
        {
            Debug.Log($"coreSlotDisks contains {coreSlotDisks.Length} slots: {string.Join(", ", System.Linq.Enumerable.Select(coreSlotDisks, s => s.name))}");
        }
        disk1.SetRotation(disk1Direction);
        disk2.SetRotation(disk2Direction);
        disk3.SetRotation(disk3Direction);
        lastSpinDirections = new int[3]; // Initialize spin direction cache
    }
}