using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OpenSecondFloorEvent : MonoBehaviour
{
    Camera mainCamera; // 메인 카메라

    [SerializeField]
    Camera targetCamera; // 목표 카메라

    [SerializeField]
    TriggerArea triggerArea; // Trigger Area

    [SerializeField]
    FadePanel fadePanel; // 페이드 판넬

    [SerializeField]
    List<GameObject> targetObjects; // 레이저의 목표 오브젝트

    [SerializeField]
    GameObject laserPointer; // 레이저 포인터

    [SerializeField]
    float laserMoveLength; // 레이저 이동 길이

    Vector3 onPos;
    Vector3 offPos;
    Quaternion initRot;

    [SerializeField]
    float laserMoveDuration; // 레이저 이동 소요 시간

    [SerializeField]
    float stepInterval; // 각 스텝 사이의 시간 간격

    bool isPlaying = false; // 이벤트 재생 상태

    float currentTime = 0f;

    void Awake() {
        onPos = laserPointer.transform.position;
        offPos = laserPointer.transform.position + Vector3.up * laserMoveLength;
        initRot = laserPointer.transform.rotation;
    }

    public void PlayEvent() {
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        
        if(mainCamera && !isPlaying) {
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

        // 레이져 등장
        laserPointer.transform.position = new(laserPointer.transform.position.x, offPos.y, laserPointer.transform.position.z);
        laserPointer.SetActive(true);

        while(laserPointer.transform.position.y > onPos.y) {
            currentTime += Time.deltaTime;
            Vector3 newPos = Vector3.Lerp(offPos, onPos, currentTime / laserMoveDuration);
            if(newPos.y < onPos.y) {
                laserPointer.transform.position = new(newPos.x, onPos.y, newPos.z);
            }
            else {
                laserPointer.transform.position = newPos;
            }
            
            yield return null;
        }
        currentTime = 0f;

        yield return new WaitForSeconds(stepInterval);

        // 타겟을 향해 조준 -> 발사
        foreach(GameObject targetObj in targetObjects) {
            if(!targetObj.activeSelf || targetObj.IsDestroyed()) continue;
            
            // 레이져의 기울기를 계산. 만약에 기울기가 일정 수준 이상 커질것 같다면 해당 오브젝트는 일단 넘긴다.
            float dx = targetObj.transform.position.x - laserPointer.transform.position.x;
            float dy = targetObj.transform.position.y - laserPointer.transform.position.y;
            float dz = targetObj.transform.position.z - laserPointer.transform.position.z;

            float slope;
            if(dx == 0 && dz == 0) {
                slope = (dy < 0 ? -1 : 1) * 1000000000;
            }
            else {
                slope = dy / Mathf.Sqrt(dx * dx + dz * dz);
            }

            Debug.Log(targetObj.transform.position);
            Debug.Log(laserPointer.transform.position);
            Debug.Log(slope);
            if(slope > -0.001f) continue;

            // 조준
            Quaternion targetRot = Quaternion.LookRotation(targetObj.transform.position - laserPointer.transform.position);
            Quaternion beforeRot = laserPointer.transform.rotation;

            while(true) {
                currentTime += Time.deltaTime;

                if(currentTime >= laserMoveDuration) {
                    laserPointer.transform.rotation = targetRot;
                    break;
                }
                else {
                    Quaternion newRot = Quaternion.Lerp(beforeRot, targetRot, currentTime / laserMoveDuration);
                    laserPointer.transform.rotation = newRot;
                }
                
                yield return null;
            }
            currentTime = 0f;
            
            yield return new WaitForSeconds(stepInterval);
            
            // 발사
            laserPointer.GetComponent<LaserPointer>().Shoot();

            yield return new WaitForSeconds(stepInterval);
        }

        // 원래 회전 상태로 복귀
        Quaternion befRot = laserPointer.transform.rotation;
        while(true) {
            currentTime += Time.deltaTime;

            if(currentTime >= laserMoveDuration) {
                laserPointer.transform.rotation = initRot;
                break;
            }
            else {
                Quaternion newRot = Quaternion.Lerp(befRot, initRot, currentTime / laserMoveDuration);
                laserPointer.transform.rotation = newRot;
            }
            
            yield return null;
        }
        currentTime = 0f;

        // 레이져 퇴장
        while(laserPointer.transform.position.y < offPos.y) {
            currentTime += Time.deltaTime;
            Vector3 newPos = Vector3.Lerp(onPos, offPos, currentTime / laserMoveDuration);
            if(newPos.y > offPos.y) {
                laserPointer.transform.position = new(newPos.x, offPos.y, newPos.z);
            }
            else {
                laserPointer.transform.position = newPos;
            }
            
            yield return null;
        }
        currentTime = 0f;

        laserPointer.SetActive(false);

        yield return new WaitForSeconds(stepInterval);

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