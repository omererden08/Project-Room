using UnityEngine;
using DG.Tweening;
public class LatestGear : MonoBehaviour
{
    public bool clockwise;
    public void StartSpin()
    {
        transform.DORotate(new Vector3(clockwise ? 360 : -360,0 , 0), 5f, RotateMode.LocalAxisAdd)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart)
                .SetRelative();
    }
}
