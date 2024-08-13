using UnityEngine;
using TMPro;

public class TriggerArea : MonoBehaviour
{
    public Camera mainCamera;
    public Transform screenViewTransform;
    public Transform originalViewTransform; // 카메라의 원래 위치와 회전
    public MonoBehaviour cameraControlScript; // 카메라 제어 스크립트 참조
    public PlayerController playerController; // PlayerController 참조
    public ScreenUIManager screenUIManager; // ScreenUIManager 참조
    public GameObject interactionPrompt; // Interaction prompt 참조
    private bool isPlayerInRange = false;
    private bool isInteracting = false;

    void Start()
    {
        if (mainCamera == null) Debug.LogError("mainCamera is not assigned.");
        if (screenViewTransform == null) Debug.LogError("screenViewTransform is not assigned.");
        if (originalViewTransform == null) Debug.LogError("originalViewTransform is not assigned.");
        if (cameraControlScript == null) Debug.LogError("cameraControlScript is not assigned.");
        if (playerController == null) Debug.LogError("playerController is not assigned.");
        if (screenUIManager == null) Debug.LogError("screenUIManager is not assigned.");
        if (interactionPrompt == null) Debug.LogError("interactionPrompt is not assigned.");

        interactionPrompt.SetActive(false); // 시작 시 텍스트 비활성화
    }

    void Update()
    {
        if (mainCamera == null || screenViewTransform == null || originalViewTransform == null || playerController == null || screenUIManager == null || interactionPrompt == null)
        {
            return;
        }

        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
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

    public bool IsInteracting() { return isInteracting; }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            interactionPrompt.SetActive(true); // 텍스트 활성화
            Debug.Log("텍스트 활성화");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            interactionPrompt.SetActive(false); // 텍스트 비활성화
            Debug.Log("텍스트 비활성화");

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
        playerController.EnterInteractionMode();
        screenUIManager.ShowScreenUI(); // UI 활성화
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
        playerController.ExitInteractionMode();
        screenUIManager.HideScreenUI(); // UI 비활성화
        interactionPrompt.SetActive(false); // 텍스트 비활성화
        isInteracting = false;
        Debug.Log("상호작용 종료");
    }

    void LockCameraControl()
    {
        // 카메라 제어 비활성화
        if (cameraControlScript != null)
        {
            cameraControlScript.enabled = false;
        }
        Cursor.lockState = CursorLockMode.None;
    }

    void UnlockCameraControl()
    {
        // 카메라 제어 활성화
        if (cameraControlScript != null)
        {
            cameraControlScript.enabled = true;
        }
        // 커서 잠금 상태 설정 부분을 제거합니다.
        // Cursor.lockState = CursorLockMode.Locked;
    }
}