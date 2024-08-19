using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Fusion;
using UnityEngine;

public class LiquefactionDoorController : NetworkBehaviour
{
    public bool isClosed = false;

    public void ToggleDoor()
    {
        isClosed = !isClosed;
        UpdateDoorVisual();
    }

    private void UpdateDoorVisual()
    {
        if (isClosed)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 100, 0);
        }
    }

    public override void FixedUpdateNetwork()
    {
        UpdateDoorVisual();
    }
}
