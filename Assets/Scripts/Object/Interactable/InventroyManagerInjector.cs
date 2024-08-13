using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventroyManagerInjector : MonoBehaviour
{
    public Collectable[] collectables;
    public Putable[] putables;

    private InventoryManager inventoryManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Transform player = other.transform;

            Transform inventroy = player.Find("[BuildingBlock] Camera Rig/TrackingSpace/CenterEyeAnchor/MainCanvas/Inventory");

            if (inventroy != null)
            {
                inventoryManager = inventroy.GetComponent<InventoryManager>();

                foreach (Collectable collectable in collectables)
                {
                    collectable.InjectInventoryManager(inventoryManager);
                }

                foreach (Putable putable in putables)
                {
                    putable.InjectInventoryManager(inventoryManager);
                }
            }
        }
    }
}
