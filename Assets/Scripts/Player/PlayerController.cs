using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerController : MonoBehaviour
{
    // 스피드 조정 변수
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;
    [SerializeField]
    private float sitSpeed;

    private float applySpeed;

    [SerializeField]
    private float jumpForce;

    // 상태 변수
    private bool isWalk = false;
    private bool isRun = false;
    private bool isSit = false;
    private bool isGround = true;
    private bool isWalkBackwards = false;

    // 상호작용 모드 상태 변수
    private bool isInteracting = false;

    // 움직임 체크 변수
    private Vector3 lastPos;

    // 앉았을때, 얼마나 앉을지 결정하는 변수
    [SerializeField]
    private float sitPosY;
    private float originPosY;
    private float applySitPosY;

    // 땅 착지 여부
    private CapsuleCollider capsuleCollider;

    // 민감도
    [SerializeField]
    private float lookSensitivity;

    // 카메라 
    [SerializeField]
    private float cameraRotationLimit;
    private float currentCameraRotationX = 0f;

    // 필요한 컴포넌트
    [SerializeField]
    private Camera theCamera;
    [SerializeField] private ActionBasedController leftController;
    [SerializeField] private ActionBasedController rightController;
    private Rigidbody myRigid;

    // 애니메이터 컴포넌트
    private Animator animator;

    // RiggingManager 참조 추가
    private RiggingManager riggingManager;

    // 머리 회전을 동기화할 트랜스폼 추가
    [SerializeField]
    private Transform headTransform;

    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        myRigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        applySpeed = walkSpeed;
        riggingManager = GetComponent<RiggingManager>();
        originPosY = theCamera.transform.localPosition.y;
        applySitPosY = originPosY;
    }

    void Update()
    {
        if (!isInteracting)
        {
            IsGround();
            TryJump();
            TryRun();
            TrySit();
            Move();
            CharacterRotation();
            CameraRotation();
            UpdateAnimator();
        }

        // RiggingManager 업데이트
        if (riggingManager != null)
        {
            riggingManager.UpdateRigging();
        }

        // 머리 회전 동기화
        SyncHeadRotation();
    }

    private void SyncHeadRotation()
    {
        if (headTransform != null && theCamera != null)
        {
            headTransform.position = theCamera.transform.position; // 머리 위치 동기화
            headTransform.rotation = theCamera.transform.rotation; // 머리 회전 동기화
        }
    }

    private void TrySit()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Sit();
        }
    }

    private void Sit()
    {
        isSit = !isSit;

        if (isSit)
        {
            applySpeed = sitSpeed;
            applySitPosY = sitPosY;
        }
        else
        {
            applySpeed = walkSpeed;
            applySitPosY = originPosY;
        }
        StartCoroutine(SitCoroutine());
    }

    IEnumerator SitCoroutine()
    {
        float startY = theCamera.transform.localPosition.y;
        float elapsedTime = 0f;
        float duration = 0.3f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newY = Mathf.Lerp(startY, applySitPosY, elapsedTime / duration);
            theCamera.transform.localPosition = new Vector3(theCamera.transform.localPosition.x, newY, theCamera.transform.localPosition.z);
            yield return null;
        }
        theCamera.transform.localPosition = new Vector3(theCamera.transform.localPosition.x, applySitPosY, theCamera.transform.localPosition.z);
    }

    private void IsGround()
    {
        isGround = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y + 0.1f);
    }

    private void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            Jump();
        }
    }

    private void Jump()
    {
        if (isSit)
            Sit();
        myRigid.velocity = transform.up * jumpForce;
        animator.SetTrigger("Jump");
    }

    private void TryRun()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Running();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            RunningCancel();
        }
    }

    private void Running()
    {
        if (isSit)
            Sit();
        isRun = true;
        applySpeed = runSpeed;
    }

    private void RunningCancel()
    {
        isRun = false;
        applySpeed = walkSpeed;
    }

    private void Move()
    {
        float moveDirX = Input.GetAxisRaw("Horizontal");
        float moveDirZ = Input.GetAxisRaw("Vertical");

        // 앞 방향 이동 우선
        if (moveDirZ > 0)
        {
            moveDirX *= 0.5f;
        }

        Vector3 moveHorizontal = transform.right * moveDirX;
        Vector3 moveVertical = transform.forward * moveDirZ;

        Vector3 moveDir = (moveHorizontal + moveVertical).normalized;

        Vector3 velocity = moveDir * applySpeed;
        myRigid.MovePosition(transform.position + velocity * Time.deltaTime);

        isWalk = (moveDirX != 0 || moveDirZ > 0);
        isWalkBackwards = (moveDirZ < 0);

        if (isRun)
        {
            isWalk = false;
        }

        if (isWalk || isRun)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.01f);
        }
    }

    private void UpdateAnimator()
    {
        animator.SetBool("Walk", isWalk);
        animator.SetBool("Run", isRun);
        animator.SetBool("isWalkBackwards", isWalkBackwards);
    }

    private void CharacterRotation()
    {
        float yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 characterRotationY = new Vector3(0f, yRotation, 0f) * lookSensitivity;
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(characterRotationY));
    }

    private void CameraRotation()
    {
        float xRotation = Input.GetAxisRaw("Mouse Y");
        float cameraRotationX = xRotation * lookSensitivity;
        currentCameraRotationX -= cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }

    // 상호작용 모드 진입/종료 함수 추가
    public void EnterInteractionMode()
    {
        isInteracting = true;
    }

    public void ExitInteractionMode()
    {
        isInteracting = false;
    }
}
