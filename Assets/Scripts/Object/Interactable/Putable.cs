using Meta.WitAi;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Putable : MonoBehaviour
{
    public string objectName;
    public TextMeshProUGUI guideText;
    public InventoryManager inventoryManager;
    public Transform putItemPosition;
    public Collectable putItem;
    private void OnMouseEnter()
    {
        ShowText();
    }

    private void OnMouseExit()
    {
        HideText();
    }

    public void ShowText()
    {
        if (inventoryManager.pickedItemIndex != -1)
        {
            Collectable obj = inventoryManager.GetItemByIndex(inventoryManager.pickedItemIndex);
            guideText.text = string.Format("\"{0}\"을/를 \"{1}\"에 두기", obj.Name, objectName);
            guideText.gameObject.SetActive(true);
        }
    }

    public void HideText()
    {
        if (inventoryManager.pickedItemIndex != -1)
        {
            guideText.text = "";
            guideText.gameObject.SetActive(false);
        }
    }

    public void PutItem()
    {
        if (inventoryManager.pickedItemIndex != -1)
        {
            putItem = inventoryManager.PutItem(putItemPosition);
            guideText.gameObject.SetActive(false);
        }
    }

    public void RemovePutItem()
    {
        putItem = null;
    }
}
