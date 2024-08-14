using Oculus.Interaction.Surfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MixerCoverController : MonoBehaviour
{
    public bool isClosed = false;
    public Transform leftCover;
    public Transform rightCover;
    public Vector3 distance = new Vector3(0, 0, -0.5f);

    private float range = 5f;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, range))
            {
                if (hit.transform == transform)
                {
                    ToggleDoor();
                }
            }
        }
    }

    public void ToggleDoor()
    {
        isClosed = !isClosed;
        if (isClosed)
        {
            leftCover.position += distance;
            rightCover.position -= distance;
        }
        else
        {
            leftCover.position -= distance;
            rightCover.position += distance;
        }
    }
}
