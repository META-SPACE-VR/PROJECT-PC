using UnityEngine;

public class OVRPlayerControllerWithAnimation : MonoBehaviour
{
    public OVRPlayerController ovrPlayerController;  // OVRPlayerController 참조
    public Animator animator;  // Animator 참조
    public Transform avatarTransform;  // 아바타의 Transform 참조
    public InventoryManager inventoryManager; // 인벤토리 참조
    public GameObject rig;

    private void Start() {
        rig.SetActive(true);
        ovrPlayerController.enabled = true;
        
    }

    private void Update()
    {
        // 플레이어의 CharacterController에서 속도 벡터를 가져옵니다.
        Vector3 velocity = ovrPlayerController.GetComponent<CharacterController>().velocity;

        // 이동 중인지 확인
        bool isWalking = velocity.magnitude > 0.1f;

        // 달리기 버튼이 눌렸는지 확인
        bool isRunning = OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) || 
                         Input.GetKey(KeyCode.LeftShift) || 
                         Input.GetKey(KeyCode.RightShift);

        // 점프 버튼이 눌렸는지 확인
        bool isJumping = OVRInput.GetDown(OVRInput.RawButton.X) ||
                        Input.GetKey(KeyCode.Space); 

        // 애니메이터의 "Walk", "Run", 및 "Jump" 파라미터를 설정
        animator.SetBool("Walk", isWalking && !isRunning);
        animator.SetBool("Run", isWalking && isRunning);

        if (isJumping)
        {
            animator.SetTrigger("Jump"); // 점프 애니메이션 트리거
            ovrPlayerController.Jump(); // 점프 수행
        }

        // 플레이어의 회전 값을 가져와서 아바타에 적용
        Quaternion playerRotation = ovrPlayerController.transform.rotation;
        avatarTransform.rotation = playerRotation;

        // 아이템 버리기 (X)
        if (inventoryManager.pickedItemIndex != -1 && OVRInput.GetDown(OVRInput.RawButton.Y))
        {
            Vector3 dropPosition = avatarTransform.position + avatarTransform.forward * 2f + Vector3.up * 2.5f;
            inventoryManager.DropItem(inventoryManager.pickedItemIndex, dropPosition);
        }
        // 아이템 자세히 보기 (A)
        if (inventoryManager.pickedItemIndex != -1 && OVRInput.GetDown(OVRInput.RawButton.A))
        {
            inventoryManager.ZoomItem();
        }
        // 아이템 자세히 보기 취소 (B)
        if (inventoryManager.isItemZoomed && OVRInput.GetDown(OVRInput.RawButton.B))
        {
            inventoryManager.UnzoomItem();
        }
        // 아이템 선택 취소 (B)
        if (!inventoryManager.isItemZoomed && inventoryManager.pickedItemIndex != -1 && OVRInput.GetDown(OVRInput.RawButton.B))
        {
            inventoryManager.DeselectItem();
        }
    }
}
