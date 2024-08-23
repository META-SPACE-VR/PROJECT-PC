using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineStart : MonoBehaviour
{
    public NPCInteraction npcInteraction;
    public Transform bedTransform;
    public float wheelDistance;
    public GameObject wheelchair;
    public Animator machineAnimator;
    public KeyCode interactionKey;
    public GameObject interactionPrompt; // Interaction prompt 참조

    // Update is called once per frame
    void Update()
    {
        // NPC와 휠체어가 가까이 있는지 확인
        if (IsNearby() && npcInteraction.isSittingInWheelchair)
        {
            // 가까이 있으면 상호작용 프롬프트 활성화
            interactionPrompt.SetActive(true);

            // E 키를 눌렀을 때 onMachineStart 메서드 호출
            if (Input.GetKeyDown(interactionKey))
            {
                onMachineStart();
            }
        }
        if (!IsNearby())
        {
            interactionPrompt.SetActive(false);
        }

    }

    private bool IsNearby()
    {
        float distance = Vector3.Distance(transform.position, wheelchair.transform.position);
        return distance <= wheelDistance;
    }

    private void onMachineStart()
    {
        if (npcInteraction.isSittingInWheelchair && IsNearby())
        {
            machineAnimator.SetTrigger("Machine_Start");

            // NPC를 침대 위치로 이동
            npcInteraction.LayOnBed(bedTransform);

            // 상호작용 프롬프트 비활성화
            interactionPrompt.SetActive(false);
        }
        else
        {
            Debug.Log("아직 할당 안됨");
        }
    }
}
