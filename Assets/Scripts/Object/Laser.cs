using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    // Line Renderer
    [SerializeField]
    LineRenderer lineRenderer;

    private void OnEnable() {
        lineRenderer.SetPosition(0, transform.position);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, LayerMask.GetMask("Destructible")))
        {
            Debug.Log(hit.collider.gameObject.name);
            if (hit.collider)
            {
                lineRenderer.SetPosition(1, hit.point);
                
                DestructibleRock dr = hit.collider.gameObject.GetComponent<DestructibleRock>();
                if(dr) {
                    dr.Explode();
                }
            }
        }
        else lineRenderer.SetPosition(1, transform.forward*5000);
    }
}
