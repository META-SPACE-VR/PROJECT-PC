using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    public TextMeshProUGUI guideText;
    public InventoryManager inventoryManager;
    public float range = 5f;
    public Collectable curCollectable = null;
    public Putable curPutable = null;

    private void Update()
    {   
        // 레이 캐스팅
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range))
        {
            Collectable collectable = hit.collider.GetComponent<Collectable>();
            
            if (collectable != null)
            {
                curCollectable = collectable;

                // 아이템 줍기
                if (Input.GetMouseButtonDown(0))
                {
                    collectable.Collect();
                }
            }

            Putable putable = hit.collider.GetComponent<Putable>();

            if (putable != null)
            {
                curPutable = putable;

                // 아이템 놓기
                if (Input.GetMouseButtonDown(0))
                {
                    putable.PutItem();
                }
            }
            else if (inventoryManager.pickedItemIndex != -1)
            {
               if (Input.GetMouseButtonDown(0))
               {
                    // 아이템 버리기
                    Vector3 dropPosition = transform.position + transform.forward * 2f + Vector3.up * 2.5f;
                    inventoryManager.DropItem(inventoryManager.pickedItemIndex, dropPosition);
               }
            }

            if (collectable == null && putable == null)
            {
                HideText();
            }
        }
    }

    private void HideText()
    {
        guideText.text = "";
        guideText.gameObject.SetActive(false);
    }
}
