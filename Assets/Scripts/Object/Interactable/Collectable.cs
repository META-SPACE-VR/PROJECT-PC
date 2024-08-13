using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public GameObject Prefab;
    public string Name;
    public Sprite Icon;
    
    public TextMeshProUGUI guideText;
    public InventoryManager InventoryManager;

    public void InjectInventoryManager(InventoryManager manager)
    {
        InventoryManager = manager;
    }

    public void InjectGuideText(TextMeshProUGUI textMesh)
    {
        guideText = textMesh;
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
        InventoryManager.AddItem(this, gameObject);
        guideText.gameObject.SetActive(false);
    }
}
