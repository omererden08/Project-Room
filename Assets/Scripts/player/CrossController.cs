using UnityEngine;

public class CrossController : MonoBehaviour
{
    public GameObject ch;

    void Start()
    {
        EvntManager.StartListening("DisableCh", DisableCh);
        EvntManager.StartListening("EnableCh", EnableCh);
    }

    public void DisableCh()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        ch.SetActive(false);
    }

    public void EnableCh()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        ch.SetActive(true);
    }
}
