using UnityEngine;

public class CrossController : MonoBehaviour
{
    public GameObject ch;
    void Start()
    {
        ch = GameObject.FindGameObjectWithTag("crosshair");
        EvntManager.StartListening("DisableCh", DisableCh);
        EvntManager.StartListening("EnableCh", EnableCh);
    }
    public void DisableCh()
    {
        ch.SetActive(false);
    }
    public void EnableCh()
    {
        ch.SetActive(true);
    }
}
