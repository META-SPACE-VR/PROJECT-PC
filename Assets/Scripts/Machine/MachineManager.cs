using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineManager : MonoBehaviour
{
    public GameObject scalpel;         // 메스
    public GameObject electrocautery;  // 전기 소작기
    public GameObject suction;         // 흡입기

    public GameObject leftArm;         // 왼쪽 팔
    public GameObject rightArm;        // 오른쪽 팔
    public GameObject centerArm;       // 중앙 팔

    private Transform scalpelOriginalParent;
    private Transform electrocauteryOriginalParent;
    private Transform suctionOriginalParent;

    void Start()
    {
        scalpelOriginalParent = scalpel.transform.parent;
        electrocauteryOriginalParent = electrocautery.transform.parent;
        suctionOriginalParent = suction.transform.parent;

        AttachTool(scalpel, leftArm);
        AttachTool(electrocautery, rightArm);
        AttachTool(suction, centerArm);
    }

    void AttachTool(GameObject tool, GameObject arm)
    {
        tool.transform.SetParent(arm.transform);
        tool.transform.localPosition = Vector3.zero; // 필요에 따라 위치 조정
        tool.transform.localRotation = Quaternion.identity; // 필요에 따라 회전 조정
    }

    void Update()
    {
        // 팔의 움직임에 따라 도구의 위치와 회전을 업데이트합니다.
        if (leftArm != null && scalpel != null)
        {
            scalpel.transform.position = leftArm.transform.position;
            scalpel.transform.rotation = leftArm.transform.rotation;
        }

        if (rightArm != null && electrocautery != null)
        {
            electrocautery.transform.position = rightArm.transform.position;
            electrocautery.transform.rotation = rightArm.transform.rotation;
        }

        if (centerArm != null && suction != null)
        {
            suction.transform.position = centerArm.transform.position;
            suction.transform.rotation = centerArm.transform.rotation;
        }
    }

    void Destroy()
    {
        // 게임 오브젝트가 파괴되거나 씬이 전환될 때 도구를 원래 부모로 복원합니다.
        scalpel.transform.SetParent(scalpelOriginalParent);
        electrocautery.transform.SetParent(electrocauteryOriginalParent);
        suction.transform.SetParent(suctionOriginalParent);
    }
}
