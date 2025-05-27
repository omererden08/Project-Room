using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{

    [SerializeField] private Volume volume;
    [SerializeField] private GameObject pauseObject;
    private DepthOfField dof;

    private void Start()
    {
        pauseObject.gameObject.SetActive(false);
        if (volume.profile.TryGet(out dof))
        {
            // �lk durumda kapal� ba�lat
            dof.active = false;
        }
        else
        {
            Debug.LogWarning("DepthOfField bulunamad�.");
        }
    }


    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (dof.active)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            FadeManager.Instance.FadeWhite("MainMenu");
        }
    }

    void PauseGame()
    {
        EnableBlur(0.1f, 3f);
        pauseObject.gameObject.SetActive(true);
        EvntManager.TriggerEvent("DisableCh"); // Oyun pause olacak
        EvntManager.TriggerEvent("pause"); // Oyun pause olacak
        EvntManager.TriggerEvent("pauseTimer"); // Oyun pause olacak
        Time.timeScale = 0f;

    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        DisableBlur();
        pauseObject.gameObject.SetActive(false);
        EvntManager.TriggerEvent("EnableCh"); // Oyun pause olacak
        EvntManager.TriggerEvent("pause");
        EvntManager.TriggerEvent("pauseTimer"); // Oyun pause olacak
    }


    public void EnableBlur(float nearStart = 0.1f, float nearEnd = 3f)
    {
        if (dof != null)
        {
            dof.active = true;
            dof.mode.value = DepthOfFieldMode.Gaussian;
            dof.gaussianStart.value = nearStart;
            dof.gaussianEnd.value = nearEnd;
            dof.gaussianMaxRadius.value = 4f; // Bulan�kl�k miktar�
        }
    }

    public void DisableBlur()
    {
        if (dof != null)
        {
            dof.active = false;
        }
    }
}


