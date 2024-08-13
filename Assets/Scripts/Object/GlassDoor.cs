using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassDoor : MonoBehaviour
{
    public bool isClosed = true;

    public void ToggleDoor()
    {
        isClosed = !isClosed;
        if (isClosed)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            transform.eulerAngles = new Vector3(0, -90, 0);
        }
    }
}

