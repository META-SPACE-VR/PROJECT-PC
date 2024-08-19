using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;

public class Collectable : NetworkBehaviour
{
    public GameObject Prefab;
    public string Name;
    public Sprite Icon;
    
    public TextMeshProUGUI guideText;
    public InventoryManager InventoryManager;

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
        guideText.text = string.Format("\"{0}\" 줍기", Name);
        guideText.gameObject.SetActive(true);
    }

    public void HideText()
    {
        guideText.text = "";
        guideText.gameObject.SetActive(false);
    }

    public void Collect()
    {
        if (InventoryManager.pickedItemIndex == -1)
        {
            InventoryManager.AddItem(this, gameObject);
            guideText.gameObject.SetActive(false);
        }
    }
}
