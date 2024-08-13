using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquefactionDoorController : MonoBehaviour
{
    public bool isClosed = false;

    public void ToggleDoor()
    {
        isClosed = !isClosed;
        if (isClosed)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 100, 0);
        }
    }

}
