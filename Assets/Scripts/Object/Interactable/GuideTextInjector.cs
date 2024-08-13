using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GuideTextInjector : MonoBehaviour
{
    public Collectable[] collectables;
    public Putable[] putables;

    private TextMeshProUGUI textMesh;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Transform player = other.transform;

            Transform guideText = player.Find("[BuildingBlock] Camera Rig/TrackingSpace/CenterEyeAnchor/MainCanvas/ShowText");

            if (textMesh != null)
            {
                textMesh = guideText.GetComponent<TextMeshProUGUI>();

                foreach (Collectable collectable in collectables)
                {
                    collectable.InjectGuideText(textMesh);
                }

                foreach (Putable putable in putables)
                {
                    putable.InjectGuideText(textMesh);
                }
            }
        }
    }
}
