using UnityEngine;
using DG.Tweening;
public class GearStarter : MonoBehaviour
{
    public bool clockwise;
    void Start()
    {
                    transform.DORotate(new Vector3(clockwise ? 360 : -360,0 , 0), 5f, RotateMode.LocalAxisAdd)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart)
                .SetRelative();
    }


}
