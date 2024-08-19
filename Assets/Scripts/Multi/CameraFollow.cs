using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Singleton { get; private set; }

    private Transform target;

    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            Debug.LogError("There should only ever be one instance of CameraFollow!");
        }
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            transform.SetPositionAndRotation(target.position, target.rotation);
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
