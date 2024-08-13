using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairsEvent : MonoBehaviour
{
    [SerializeField]
    GameObject rig; // Rig

    [SerializeField]
    GameObject targetPos; // Rig의 목표 위치

    [SerializeField]
    FadePanel fadePanel; // 페이드 판넬

    [SerializeField]
    Stairs stairs; // 계단

    [SerializeField]
    public OVRPlayerController playerController;

    bool isPlaying = false; // 이벤트 재생 상태

    public void PlayEvent() {
        if(!isPlaying) {
            StartCoroutine(PlayEventFlow());
        }
    }

    IEnumerator PlayEventFlow() {
        isPlaying = true;

        // 플레이어 컨트롤러 비활성화
        playerController.enabled = false;

        // Scene Fade Out
        yield return fadePanel.FadeOut();

        // 메인 카메라의 위치와 회전 상태 저장
        Vector3 rigPos = rig.transform.position;
        Quaternion rigRot = rig.transform.rotation;

        // 카메라 전환
        rig.transform.position = targetPos.transform.position;
        rig.transform.rotation = targetPos.transform.rotation;

        // Scene Fade In
        yield return fadePanel.FadeIn();

        if(!stairs.isFinished()) {
            stairs.Use();
            yield return new WaitForSeconds(stairs.GetMovingTime() * 1.1f);
        }  

        // Scene Fade Out
        yield return fadePanel.FadeOut();

        // 원래 위치로 복귀
        rig.transform.position = rigPos;
        rig.transform.rotation = rigRot;

        // Scene Fade In
        yield return fadePanel.FadeIn();

        playerController.enabled = true;

        isPlaying = false;
    }
}
