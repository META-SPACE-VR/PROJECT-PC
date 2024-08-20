using UnityEngine;
using FusionExamples.Utility;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance => Singleton<CameraFollow>.Instance;

    private Transform target;
    
    private void LateUpdate()
    {
        if (target != null)
            transform.SetPositionAndRotation(target.position, target.rotation);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
