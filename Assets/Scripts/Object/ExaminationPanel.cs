using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExaminationPanel : MonoBehaviour
{
    public Button examinationButton;
    public Button startExaminationButton;
    public NPCInteraction npcInteraction;
    public Animator bedAnimator;
    public float wheelDistance;
    public GameObject wheelchair;
    public Transform bedTransform;
    public TriggerArea triggerArea; 
    public GameObject resultPanel;
    public Animator doorAnimator;

    void Start()
    {
        examinationButton.onClick.AddListener(OnExaminationButtonClick);
        startExaminationButton.onClick.AddListener(OnStartExaminationButtonClick);

        if (resultPanel != null)
        {
            resultPanel.SetActive(false); // Initially hide the result panel
        }
    }

    void Update()
    {
        if (npcInteraction.isSittingInWheelchair)
        {
            // NPC가 휠체어에 앉아 있고, 휠체어가 범위를 벗어났을 때 홀로그램을 끕니다.
            if (resultPanel != null && resultPanel.activeSelf)
            {
                resultPanel.SetActive(false);
            }
        }
    }

    private bool IsNearby()
    {
        float distance = Vector3.Distance(transform.position, wheelchair.transform.position);
        return distance <= wheelDistance;
    }

    private void OnExaminationButtonClick()
    {
        if (npcInteraction.isSittingInWheelchair && IsNearby())
        {
            bedAnimator.SetTrigger("Bed_Out");

            // Move the patient to the bed
            npcInteraction.LayOnBed(bedTransform);

            // Exit interaction in TriggerArea
            if (triggerArea != null)
            {
                triggerArea.ExitInteraction(); // Call ExitInteraction to hide other UI elements
            }
        }
        else
        {
            Debug.Log("아직 할당 안됨");
        }
    }

    private void OnStartExaminationButtonClick()
    {
        if (npcInteraction.transform.IsChildOf(bedTransform))
        {
            bedAnimator.SetTrigger("Bed_In");
            DisplayHologramResults();
        }
    }

    private void DisplayHologramResults()
    {
        if (triggerArea != null)
        {
            triggerArea.ExitInteraction(); // Call ExitInteraction to hide other UI elements
            Destroy(triggerArea.gameObject);
        }

        if (resultPanel != null)
        {
            resultPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("Result panel is not assigned.");
        }
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("Door_Open");
        }
    }
}
