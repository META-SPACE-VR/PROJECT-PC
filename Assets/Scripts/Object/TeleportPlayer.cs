using UnityEngine;
using TMPro;
using System.Collections; // IEnumerator를 사용하기 위한 네임스페이스 추가
using Fusion;

public class TeleportPlayer : MonoBehaviour
{
    private Player playerController;
    public GameObject interactionPrompt; // Interaction prompt 참조
    private bool canTeleport = false; // 텔레포트 가능 여부
    public Animator spaceshipAnimator; // 애니메이터
    public GameObject boxColliderObject; // 콜라이더가 적용된 박스 오브젝트
    public float moveDurationUp = 6.5f; // 올라갈 때 이동에 걸리는 시간
    public float moveDurationDown = 5.0f; // 내려갈 때 이동에 걸리는 시간
    public float delayBeforeMoveUp = 0.0f; // 올라갈 때 애니메이션 후 지연 시간
    public float delayBeforeMoveDown = 3.0f; // 내려갈 때 애니메이션 후 지연 시간
    public float thresholdY = 11.0f; // 층을 구분하는 Y 좌표 임계값

    private Vector3 originalBoxColliderPosition; // 원래 박스 콜라이더 위치
    private Vector3 targetBoxColliderPosition; // 목표 박스 콜라이더 위치
    private bool isTeleporting = false; // 텔레포트 애니메이션이 진행 중인지 여부
    private float lerpTime = 0; // Lerp를 위한 시간 변수
    private float currentMoveDuration; // 현재 이동에 걸리는 시간
    private float currentDelayBeforeMove; // 현재 애니메이션 후 지연 시간

    void Start()
    {
        // interactionPrompt.SetActive(false); // 시작 시 텍스트 비활성화
        originalBoxColliderPosition = boxColliderObject.transform.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("플레이어 발견!!!");
            canTeleport = true;
            interactionPrompt.SetActive(true); // 텍스트 활성화
            playerController = other.GetComponent<Player>();
            playerController.SetCurrentTeleportPlayer(this);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("플레이어 나감!!!");
            canTeleport = false;
            interactionPrompt.SetActive(false); // 텍스트 비활성화
            playerController.ClearTeleportPlayer();
        }
    }

    public void Teleport()
    {
        if (canTeleport)
        {
            Debug.Log("텔레포트!!!");
            StartCoroutine(TeleportCoroutine());
        }
    }


    void Update()
    {
        // 텔레포트 애니메이션이 진행 중인 경우, 박스 콜라이더 이동
        if (isTeleporting)
        {
            lerpTime += Time.deltaTime / currentMoveDuration; // 시간 증가
            boxColliderObject.transform.position = Vector3.Lerp(originalBoxColliderPosition, targetBoxColliderPosition, lerpTime);

            // 애니메이션이 완료되면 상태를 리셋
            if (lerpTime >= 1)
            {
                isTeleporting = false;
                lerpTime = 0;
                // 최종 위치를 보장
                boxColliderObject.transform.position = targetBoxColliderPosition;
            }
        }
    }

    IEnumerator TeleportCoroutine()
    {
        // 박스 콜라이더의 목표 위치 설정
        float currentY = boxColliderObject.transform.position.y;

        if (currentY < thresholdY)
        {
            // 현재 위치가 1층이면 위로 11만큼 이동
            targetBoxColliderPosition = boxColliderObject.transform.position + new Vector3(0, 11f, 0);
            spaceshipAnimator.SetTrigger("LiftUp");
            currentMoveDuration = moveDurationUp;
            currentDelayBeforeMove = delayBeforeMoveUp;
        }
        else
        {
            // 현재 위치가 2층이면 아래로 11만큼 이동
            targetBoxColliderPosition = boxColliderObject.transform.position - new Vector3(0, 11f, 0);
            spaceshipAnimator.SetTrigger("LiftDown");
            currentMoveDuration = moveDurationDown;
            currentDelayBeforeMove = delayBeforeMoveDown;
        }

        // 애니메이션 재생 후 지연
        yield return new WaitForSeconds(currentDelayBeforeMove);

        // 텔레포트 애니메이션을 시작합니다
        isTeleporting = true;
        originalBoxColliderPosition = boxColliderObject.transform.position;
    }


}
