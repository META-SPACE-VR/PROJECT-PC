using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class DialController : NetworkBehaviour
{
    [Networked] public int currentNumber { get; set; } = 4;

    public int startNumber = 4;
    public int endNumber = 7;
    public float rotationAngle = 45.0f;

    public void RotateDial()
    {
        UpdateDoorVisual();
    }

    private void UpdateDoorVisual()
    {
        if (currentNumber > endNumber)
        {
            currentNumber = 1;
            transform.Rotate(0, -rotationAngle * (endNumber - 1), 0);
        }
        else
        {
            currentNumber += 1;
            transform.Rotate(0, rotationAngle, 0);
        }
    }

    public override void FixedUpdateNetwork()
    {
        UpdateDoorVisual();
    }
}
