using UnityEngine;

public enum CoreSlotStat
{
    L,
    M,
    R
}

public class CoreSlotDisk : MonoBehaviour
{
    public CoreSlotStat stat;
    [SerializeField] private DevreSystem dS;
    private bool isColliding = false;
    private float recalcInterval = 0.5f; // Recalculate every 0.5 seconds to reduce lag
    private float lastRecalcTime = 0f;

    void Start()
    {
        if (dS == null)
        {
            dS = FindObjectOfType<DevreSystem>();
            if (dS == null)
            {
                Debug.LogError("DevreSystem not found in scene!", this);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("SteamCore"))
        {
            Debug.Log($"SteamCore entered slot {name} with stat {stat}, starting smooth spin");
            isColliding = true;
            if (dS != null)
            {
                dS.CalculateSpin(this);
                dS.NotifySlotStateChanged();
                lastRecalcTime = Time.time;
            }
            else
            {
                Debug.LogWarning("DevreSystem is null, cannot calculate spin!", this);
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("SteamCore") && isColliding && Time.time >= lastRecalcTime + recalcInterval)
        {
            Debug.Log($"SteamCore still in slot {name} with stat {stat}, recalculating spin");
            if (dS != null)
            {
                dS.CalculateSpin(this);
                lastRecalcTime = Time.time;
            }
            else
            {
                Debug.LogWarning("DevreSystem is null, cannot calculate spin!", this);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("SteamCore"))
        {
            Debug.Log($"SteamCore exited slot {name} with stat {stat}, stopping smooth spin");
            isColliding = false;
            if (dS != null)
            {
                dS.CalculateSpin(this);
                dS.NotifySlotStateChanged();
                lastRecalcTime = Time.time;
            }
            else
            {
                Debug.LogWarning("DevreSystem is null, cannot calculate spin!", this);
            }
        }
    }

    public bool isActive()
    {
        Debug.Log($"Slot {name} isActive: {isColliding}");
        return isColliding;
    }
}