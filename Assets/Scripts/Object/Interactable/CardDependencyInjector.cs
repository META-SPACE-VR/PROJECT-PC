using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardDependencyInjector : MonoBehaviour
{
    public string playerJob;
    public Collectable collectable;

    private InventoryManager inventoryManager;
    private TextMeshProUGUI textMesh;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerData playerData = other.GetComponent<PlayerData>();

            if (playerData.currentJob != playerJob)
            {
                return;
            }

            Transform player = other.transform;

            Transform inventroy = player.Find("[BuildingBlock] Camera Rig/TrackingSpace/CenterEyeAnchor/MainCanvas/Inventory");
            Transform guideText = player.Find("[BuildingBlock] Camera Rig/TrackingSpace/CenterEyeAnchor/MainCanvas/ShowText");

            if (inventroy != null)
            {
                inventoryManager = inventroy.GetComponent<InventoryManager>();
                collectable.InjectInventoryManager(inventoryManager);
            }
            if (guideText != null)
            {
                textMesh = guideText.GetComponent<TextMeshProUGUI>();
                collectable.InjectGuideText(textMesh);
            }
        }
    }
}
