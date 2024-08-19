using Oculus.Interaction.Surfaces;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class MixerCoverController : NetworkBehaviour
{
    [Networked] public bool isClosed { get; set; }  = false;

    public Transform leftCover;
    public Transform rightCover;
    public Vector3 distance = new Vector3(0, 0, -0.5f);

    public void ToggleDoor()
    {
        isClosed = !isClosed;
        UpdateDoorVisual();
    }

    private void UpdateDoorVisual()
    {
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

    public override void FixedUpdateNetwork()
    {
        UpdateDoorVisual();
    }
}
