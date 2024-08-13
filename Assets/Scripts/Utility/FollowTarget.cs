using UnityEngine;

/// <summary>
/// Simple class used to have a transform following another one.
/// </summary>
public class FollowTarget : MonoBehaviour
{
	public Transform target;

    private void Update()
	{
		if (target)
		{
			transform.position = target.position;
		}
	}
}
