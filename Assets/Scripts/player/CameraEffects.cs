using UnityEngine;
using DG.Tweening;
public class CameraEffects : MonoBehaviour
{
    private Camera cam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.main;
        EvntManager.StartListening("CameraShakeLittle", CameraShakeLittle);
        EvntManager.StartListening("CameraShakeBig", CameraShakeBig);
    }

    private void CameraShakeLittle()
    {
        cam.DOShakePosition(4f, 0.05f,10,40,false);
    }
    private void CameraShakeBig()
    {
        cam.DOShakePosition(6f, 0.1f,20 ,70,false,ShakeRandomnessMode.Full);
    }
}
