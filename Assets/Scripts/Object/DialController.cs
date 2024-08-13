using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialController : MonoBehaviour
{
    public int currentNumber = 4;
    public int startNumber = 4;
    public int endNumber = 7;
    public float rotationAngle = 45.0f;

    public void RotateDial()
    {
        currentNumber += 1;
        if (currentNumber > endNumber)
        {
            currentNumber = 1;
            transform.Rotate(0, -rotationAngle * (endNumber - 1), 0);
        }
        else
        {
            transform.Rotate(0, rotationAngle, 0);
        }
    }
}
