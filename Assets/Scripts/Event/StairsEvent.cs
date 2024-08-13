using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairsEvent : MonoBehaviour
{
    [SerializeField]
    Camera mainCamera; // 메인 카메라

    [SerializeField]
    Camera targetCamera; // 목표 카메라

    [SerializeField]
    FadePanel fadePanel; // 페이드 판넬

    [SerializeField]
    Stairs stairs; // 계단

    bool isPlaying = false; // 이벤트 재생 상태

    public void PlayEvent() {
        if(!isPlaying) {
            StartCoroutine(PlayEventFlow());
        }
    }

    IEnumerator PlayEventFlow() {
        isPlaying = true;

        // Scene Fade Out
        yield return fadePanel.FadeOut();

        // 카메라 전환
        mainCamera.enabled = false;
        targetCamera.enabled = true;

        // Scene Fade In
        yield return fadePanel.FadeIn();

        if(!stairs.isFinished()) {
            stairs.Use();
            yield return new WaitForSeconds(stairs.GetMovingTime() * 1.1f);
        }  

        // Scene Fade Out
        yield return fadePanel.FadeOut();

        // 카메라 전환
        mainCamera.enabled = true;
        targetCamera.enabled = false;

        // Scene Fade In
        yield return fadePanel.FadeIn();

        isPlaying = false;
    }
}
