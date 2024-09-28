using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private float originPosY;
    private float originalHeight;
    private float targetHeight;

    // 땅 착지 여부
    private CapsuleCollider capsuleCollider;

    // 민감도
    [SerializeField]
    private float lookSensitivity;

    // 카메라 
    [SerializeField]
    private float cameraRotationLimit = 85f;
    private float currentCameraRotationX = 0f;

    // 필요한 컴포넌트
    [SerializeField]
    private Camera theCamera;
    private Rigidbody myRigid;

    // 애니메이터 컴포넌트
    private Animator animator;

    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        myRigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        applySpeed = walkSpeed;
        originPosY = theCamera.transform.localPosition.y;
        targetHeight = originalHeight;

        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (!isInteracting)
        {
            IsGround();
            TryJump();
            TryRun();
            Move();
            CharacterRotation();
            CameraRotation();
            UpdateAnimator();
        }
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

        myRigid.velocity = transform.up * jumpForce;
        animator.SetTrigger("Jump");
    }

    private void TryRun()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetAxisRaw("Vertical") > 0)
        {
            Running();
        }
        else
        {
            RunningCancel();
        }
    }

    private void Running()
    {
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
