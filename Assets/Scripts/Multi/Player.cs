using Fusion;
using Fusion.Addons.SimpleKCC;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private MeshRenderer[] modelParts;
    [SerializeField] private SimpleKCC kcc;
    [SerializeField] private Transform camTarget;
    [SerializeField] private float lookSensitivity = 0.15f;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float jumpImpulse = 10f;

    [SerializeField] private Camera playerCamera;
    [SerializeField] private Animator animator;  // Animator 추가

    private float currentSpeed;

    [Networked] private NetworkButtons PreviousButtons { get; set; }

    public override void Spawned()
    {
        kcc.SetGravity(Physics.gravity.y * 2f);

        if (HasInputAuthority)
        {
            foreach (MeshRenderer renderer in modelParts)
            {
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            }

            playerCamera = Camera.main;
            playerCamera.transform.SetParent(camTarget);
            playerCamera.transform.localPosition = Vector3.zero;
            playerCamera.transform.localRotation = Quaternion.identity;

            CameraFollow.Singleton.SetTarget(camTarget);
        }
        else
        {
            if (playerCamera != null)
            {
                playerCamera.enabled = false;
            }
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetInput input))
        {
            // 회전 처리 추가 (마우스 움직임 또는 입력)
            kcc.AddLookRotation(input.LookDelta * lookSensitivity);

            // KCC의 움직임 업데이트
            UpdateMovement(input);

            // 카메라 타겟 업데이트
            UpdateCamTarget();

            PreviousButtons = input.Buttons;
        }
    }

    public override void Render()
    {
        UpdateCamTarget();

        // NetInput 타입을 명시적으로 지정
        if (GetInput<NetInput>(out var input))
        {
            UpdateAnimation(input);
        }
    }

    private void UpdateMovement(NetInput input)
    {
        // 달리기와 걷기 속도 설정
        currentSpeed = input.Buttons.IsSet(InputButton.Run) ? runSpeed : walkSpeed;

        Vector3 worldDirection = kcc.TransformRotation * new Vector3(input.Direction.x, 0f, input.Direction.y);
        float jump = 0f;

        // 점프 입력 감지
        if (input.Buttons.WasPressed(PreviousButtons, InputButton.Jump) && kcc.IsGrounded)
        {
            jump = jumpImpulse;
        }

        kcc.Move(worldDirection.normalized * currentSpeed, jump);
    }

    private void UpdateAnimation(NetInput input)
    {
        bool isWalking = input.Direction.magnitude > 0;
        bool isRunning = input.Buttons.IsSet(InputButton.Run);
        
        animator.SetBool("Walk", isWalking && !isRunning);
        animator.SetBool("Run", isRunning);
    }

    private void UpdateCamTarget()
    {
        camTarget.localRotation = Quaternion.Euler(kcc.GetLookRotation().x, 0f, 0f);
    }
}
