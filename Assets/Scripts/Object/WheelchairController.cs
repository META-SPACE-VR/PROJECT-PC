using UnityEngine;
using TMPro;

public class WheelchairController : MonoBehaviour
{
    public float moveSpeed = 5f; // 속도 조정
    public Transform playerCamera;
    public GameObject player;
    public GameObject wheelchair;
    public TMP_Text interactionText; // 상호작용 텍스트 UI
    public float offsetZ = 1f; // 휠체어를 플레이어의 앞에 배치할 오프셋

    private bool isInteracting = false;
    private bool isPlayerInRange = false;
    private Rigidbody wheelchairRigidbody;
    private Transform originalParent;

    private float originalPlayerMoveSpeed;
    public OVRInput.Button interactionButton = OVRInput.Button.Two;

    void Start()
    {
        // 모든 공개 변수가 할당되었는지 확인하는 디버그 로그
        if (playerCamera == null)
            Debug.LogError("PlayerCamera is not assigned.");
        if (player == null)
            Debug.LogError("Player is not assigned.");
        if (wheelchair == null)
            Debug.LogError("Wheelchair is not assigned.");
        if (interactionText == null)
            Debug.LogError("InteractionText is not assigned.");

        interactionText.gameObject.SetActive(false); // 상호작용 텍스트 비활성화

        // 자식 객체에서 Rigidbody를 찾음
        wheelchairRigidbody = wheelchair.GetComponentInChildren<Rigidbody>();
        if (wheelchairRigidbody == null)
        {
            Debug.LogError("Wheelchair does not have a Rigidbody component in its children.");
        }
        else
        {
            wheelchairRigidbody.isKinematic = true; // 기본적으로 kinematic 상태 유지
        }

        originalParent = wheelchair.transform.parent; // 휠체어의 원래 부모 객체 저장
    }

    void Update()
    {
        if (isPlayerInRange && OVRInput.GetDown(interactionButton))
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

        if (isInteracting)
        {
            // 플레이어의 입력에 따라 휠체어를 이동시킴
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            Vector3 movement = playerCamera.forward * moveVertical + playerCamera.right * moveHorizontal;
            movement.y = 0; // 휠체어가 수직으로 이동하지 않도록
            player.transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            interactionText.gameObject.SetActive(true); // 상호작용 텍스트 활성화
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            interactionText.gameObject.SetActive(false); // 상호작용 텍스트 비활성화
        }
    }

    public void EnterInteraction()
    {
        isInteracting = true;
        Debug.Log("상호작용 시작");

        if (wheelchairRigidbody != null)
        {
            wheelchairRigidbody.isKinematic = true; // 휠체어를 상호작용 중에도 kinematic 상태로 유지
        }

        // 휠체어를 플레이어의 자식으로 설정하여 함께 이동하도록 함
        wheelchair.transform.SetParent(player.transform);

        // 휠체어의 방향을 플레이어의 방향과 동일하게 설정
        wheelchair.transform.rotation = player.transform.rotation;

        // 휠체어를 플레이어의 앞쪽으로 배치
        Vector3 newPosition = player.transform.position + player.transform.forward * offsetZ;
        newPosition.y = wheelchair.transform.position.y; // 휠체어의 y 좌표는 유지
        wheelchair.transform.position = newPosition;
    }

    public void ExitInteraction()
    {
        isInteracting = false;
        Debug.Log("상호작용 종료");

        if (wheelchairRigidbody != null)
        {
            wheelchairRigidbody.isKinematic = true; // 상호작용 종료 시 kinematic 상태 유지
        }

        // 휠체어를 원래 부모 객체로 되돌림
        wheelchair.transform.SetParent(originalParent);
    }
}