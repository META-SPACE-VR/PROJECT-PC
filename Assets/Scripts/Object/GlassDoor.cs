using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassDoor : MonoBehaviour
{
    public bool isClosed = true;

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
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            transform.eulerAngles = new Vector3(0, -90, 0);
        }
    }
}

