using Unity.VisualScripting;
using UnityEngine;

public class DevreSystem : MonoBehaviour
{
    public Disk[] disks;

    private Disk disk1;
    private Disk disk2;
    private Disk disk3;

    public CoreSlotDisk[] coreSlotDisks;

    [Tooltip("Initial direction: 1 for clockwise, -1 for counterclockwise")]
    public int disk1Direction;
    [Tooltip("Initial direction: 1 for clockwise, -1 for counterclockwise")]
    public int disk2Direction;
    [Tooltip("Initial direction: 1 for clockwise, -1 for counterclockwise")]
    public int disk3Direction;

    public void CalculateSpin(CoreSlotDisk triggeringSlot)
    {
        Debug.Log($"CalculateSpin called for slot {triggeringSlot.name} with stat: {triggeringSlot.stat}, isActive: {triggeringSlot.isActive()}");

        bool hasL = false;
        bool hasM = false;
        bool hasR = false;

        // Check the triggering slot
        if (triggeringSlot.isActive())
        {
            Debug.Log($"Triggering slot {triggeringSlot.name} is active with stat: {triggeringSlot.stat}");
            switch (triggeringSlot.stat)
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

        // Stop all disks to reset their state
        disk1.Stop();
        disk2.Stop();
        disk3.Stop();

        if (hasL && !hasM && !hasR)
        {
            Debug.Log("L only: Spinning disk1 (+1), disk2 (+1)");
            disk1.Spin(1);
            disk2.Spin(1);
        }
        else if (!hasL && hasM && !hasR)
        {
            Debug.Log("M only: Spinning disk1 (-1), disk2 (-1), disk3 (-1)");
            disk1.Spin(-1);
            disk2.Spin(-1);
            disk3.Spin(-1);
        }
        else if (!hasL && !hasM && hasR)
        {
            Debug.Log("R only: Spinning disk2 (+1), disk3 (+1)");
            disk2.Spin(1);
            disk3.Spin(1);
        }
        else if (hasL && hasM && !hasR)
        {
            Debug.Log("L + M: Spinning disk1 (+1), disk2 (-1), disk3 (-1)");
            disk1.Spin(1);
            disk2.Spin(-1);
            disk3.Spin(-1);
        }
        else if (!hasL && hasM && hasR)
        {
            Debug.Log("M + R: Spinning disk1 (-1), disk2 (-1), disk3 (+1)");
            disk1.Spin(-1);
            disk2.Spin(-1);
            disk3.Spin(1);
        }
        else if (hasL && !hasM && hasR)
        {
            Debug.Log("L + R: Spinning disk1 (+1), disk2 (+1), disk3 (+1)");
            disk1.Spin(1);
            disk2.Spin(1);
            disk3.Spin(1);
        }
        else if (hasL && hasM && hasR)
        {
            Debug.Log("L + M + R: Spinning disk1 (0), disk2 (0), disk3 (0)");
            disk1.Spin(0);
            disk2.Spin(0);
            disk3.Spin(0);
        }
        else
        {
            Debug.Log("No slots active: No spinning");
        }
    }

    public void Start()
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
    }
}