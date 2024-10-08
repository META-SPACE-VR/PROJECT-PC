using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Fusion;
using UnityEngine;

public class LiquefactionDoorController : MonoBehaviour
{
    public bool IsClosed { get; set; } = false;

    public void ToggleDoor()
    {
        IsClosed = !IsClosed;
        UpdateDoorVisual();
    }

    private void UpdateDoorVisual()
    {
        if (IsClosed)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 100, 0);
        }
    }

}
