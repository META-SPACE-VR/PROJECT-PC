using UnityEngine;
using TMPro;

public class TriggerArea : MonoBehaviour
{
    public Camera mainCamera;
    public Transform screenViewTransform;
    public Transform originalViewTransform; // 카메라의 원래 위치와 회전

    public ScreenUIManager screenUIManager; // ScreenUIManager 참조
    public GameObject interactionPrompt; // Interaction prompt 참조
    public GameObject otherCanvas; // 다른 캔버스 참조 (숨길/보일 캔버스)

    private bool isPlayerInRange = false;
    private bool isInteracting = false;
    public OVRInput.Button interactionButton = OVRInput.Button.One;

    void Start()
    {
        if (mainCamera == null) Debug.LogError("mainCamera is not assigned.");
        if (screenViewTransform == null) Debug.LogError("screenViewTransform is not assigned.");
        if (originalViewTransform == null) Debug.LogError("originalViewTransform is not assigned.");
        if (screenUIManager == null) Debug.LogError("screenUIManager is not assigned.");
        if (interactionPrompt == null) Debug.LogError("interactionPrompt is not assigned.");
        if (otherCanvas == null) Debug.LogError("otherCanvas is not assigned.");

        interactionPrompt.SetActive(false); // 시작 시 텍스트 비활성화
        otherCanvas.SetActive(true); // 시작 시 다른 캔버스 활성화 (필요에 따라 조정)
    }

    void Update()
    {
        if (mainCamera == null || screenViewTransform == null || originalViewTransform == null || screenUIManager == null || interactionPrompt == null || otherCanvas == null)
        {
            return;
        }

        if (isPlayerInRange && OVRInput.GetDown(interactionButton))
        {
            Debug.Log("상호작용버튼 클릭!!!");
            if (isInteracting)
            {
                ExitInteraction();
            }
            else
            {
                EnterInteraction();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("플레이어 발견!!!");
            isPlayerInRange = true;
            interactionPrompt.SetActive(true); // 텍스트 활성화
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("플레이어 나감!!!");
            isPlayerInRange = false;
            interactionPrompt.SetActive(false); // 텍스트 비활성화
        }
    }

    public void EnterInteraction()
    {
        // 상호작용 위치로 이동
        mainCamera.transform.position = screenViewTransform.position;
        mainCamera.transform.rotation = screenViewTransform.rotation;

        // 카메라가 모니터를 바라보게 함
        mainCamera.transform.LookAt(screenViewTransform);

        LockCameraControl();
        screenUIManager.ShowScreenUI(); // UI 활성화
        otherCanvas.SetActive(false); // 다른 캔버스 숨기기
        interactionPrompt.SetActive(false); // 텍스트 비활성화
        isInteracting = true;
        Debug.Log("상호작용 시작");
    }

    public void ExitInteraction()
    {
        // 원래 위치로 돌아가기
        mainCamera.transform.position = originalViewTransform.position;
        mainCamera.transform.rotation = originalViewTransform.rotation;
        UnlockCameraControl();
        screenUIManager.HideScreenUI(); // UI 비활성화
        otherCanvas.SetActive(true); // 다른 캔버스 다시 활성화
        interactionPrompt.SetActive(false); // 텍스트 비활성화
        isInteracting = false;
        Debug.Log("상호작용 종료");
    }

    void LockCameraControl()
    {
        // 카메라 제어 비활성화
        Cursor.lockState = CursorLockMode.None;
    }

    void UnlockCameraControl()
    {
        // 카메라 제어 활성화
        // 커서 잠금 상태 설정 부분을 제거합니다.
    }
}
