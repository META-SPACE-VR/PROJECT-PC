using Fusion;
using UnityEngine;
using TMPro;

public class WheelchairController : NetworkBehaviour
{
    // [Networked] public bool isInteracting { get; set; } = false;
    [Networked] public Vector3 Position { get; set; }
    [Networked] public Quaternion Rotation { get; set; }


    public float moveSpeed = 5f; // 속도 조정
    // public Transform playerCamera;
    public GameObject player;
    public GameObject wheelchair;
    public TMP_Text interactionText; // 상호작용 텍스트 UI
    public float offsetZ = 1f; // 휠체어를 플레이어의 앞에 배치할 오프셋

    public bool isInteracting = false;
    private Rigidbody wheelchairRigidbody;
    private Transform originalParent;

    private float originalPlayerMoveSpeed;

    void Start()
    {
        // 모든 공개 변수가 할당되었는지 확인하는 디버그 로그
        if (player == null)
            Debug.LogError("Player is not assigned.");
        if (wheelchair == null)
            Debug.LogError("Wheelchair is not assigned.");
        if (interactionText == null)
            Debug.LogError("InteractionText is not assigned.");

        interactionText.gameObject.SetActive(false); // 상호작용 텍스트 비활성화

        // 자식 객체에서 Rigidbody를 찾음
        wheelchairRigidbody = wheelchair.GetComponent<Rigidbody>();
        if (wheelchairRigidbody != null)
        {
            wheelchairRigidbody.isKinematic = true; // 물리적 힘 비활성화
        }

        originalParent = wheelchair.transform.parent; // 휠체어의 원래 부모 객체 저장
    }

    // void Update()
    // {
    //     if (isPlayerInRange && Input.GetKeyDown(KeyCode.R))
    //     {
    //         if (isInteracting)
    //         {
    //             ExitInteraction();
    //         }
    //         else
    //         {
    //             EnterInteraction();
    //         }
    //     }

    // if (isInteracting)
    // {
    //     // 플레이어의 입력에 따라 휠체어를 이동시킴
    //     float moveHorizontal = Input.GetAxis("Horizontal");
    //     float moveVertical = Input.GetAxis("Vertical");

    //     Vector3 movement = playerCamera.forward * moveVertical + playerCamera.right * moveHorizontal;
    //     movement.y = 0; // 휠체어가 수직으로 이동하지 않도록
    //     player.transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);
    // }
    // }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player playerController = other.GetComponent<Player>();

            if (playerController != null)
            {
                player = other.gameObject; // 플레이어 변수를 자동으로 할당
                playerController.SetCurrentWheelchair(this);
                interactionText.gameObject.SetActive(true); // 상호작용 텍스트 활성화
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player playerController = other.GetComponent<Player>();

            if (playerController != null)
            {
                playerController.ClearCurrentWheelchair();
                interactionText.gameObject.SetActive(false); // 상호작용 텍스트 비활성화
            }
        }
    }

    public void EnterInteraction()
    {
        if (wheelchairRigidbody != null)
        {
            wheelchairRigidbody.isKinematic = true; // 물리적 힘 비활성화
            wheelchairRigidbody.useGravity = true; // 중력 비활성화
        }

        isInteracting = true;
        Debug.Log("상호작용 시작");

        // 휠체어를 플레이어의 자식으로 설정하여 함께 이동하도록 함
        wheelchair.transform.SetParent(player.transform);

        UpdateWheelChairPosition();
        interactionText.gameObject.SetActive(false); // 상호작용 텍스트 비활성화


        // // 초기 회전값을 설정
        // wheelchair.transform.localRotation = Quaternion.Euler(-90, 0, 0);

        // // 휠체어를 플레이어의 앞쪽으로 배치
        // wheelchair.transform.localPosition = new Vector3(0, 0, offsetZ);

    }

    public override void FixedUpdateNetwork()
    {
        if (isInteracting && player != null)
        {
            Vector3 playerPosition = player.transform.position;
            Quaternion playerRotation = player.transform.rotation;

            Position = playerPosition + player.transform.forward * 2f;
            Rotation = playerRotation * Quaternion.Euler(-90, 0, 0); // 처음에 설정했던 -90도 회전을 유지

            wheelchair.transform.position = Position;
            wheelchair.transform.rotation = Rotation;

            UpdateWheelChairPosition();
        }

    }


    public void UpdateWheelChairPosition()
    {
        Vector3 playerPosition = player.transform.position;
        Quaternion playerRotation = player.transform.rotation;

        // 휠체어를 플레이어 앞쪽에 배치
        Position = playerPosition + player.transform.forward * 2f;
        Rotation = playerRotation * Quaternion.Euler(-90, 0, 0); // 처음에 설정했던 -90도 회전을 유지

        wheelchair.transform.position = Position;
        wheelchair.transform.rotation = Rotation;
    }



    public void ExitInteraction()
    {
        if (wheelchairRigidbody != null)
        {
            wheelchairRigidbody.isKinematic = true; // 물리적 힘 활성화
            wheelchairRigidbody.useGravity = true; // 중력 활성화
        }
        // 휠체어의 부모를 해제
        wheelchair.transform.SetParent(null);

        isInteracting = false; // 상호작용 종료
        Debug.Log("상호작용 종료");
    }

}
