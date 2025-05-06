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
        ch.SetActive(false);
        Cursor.visible = true;

    }
    public void EnableCh()
    {
        Cursor.visible = false;

        ch.SetActive(true);
    }
}
