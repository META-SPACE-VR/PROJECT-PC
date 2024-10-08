using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MixerCoverPartController : MonoBehaviour
{
    public Transform leftCover;
    public Transform rightCover;
    public MixerCoverController controller;
    public Vector3 distance = new Vector3(0, 0, -0.5f);

    public void ToggleDoor()
    {
        controller.ToggleClose();
        UpdateDoorVisual();
    }

    private void UpdateDoorVisual()
    {
        if (controller.IsClosed)
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
