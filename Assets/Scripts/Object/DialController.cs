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
                    RotateDial();
                }
            }
        }
    }

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
