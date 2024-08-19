using Fusion;
using Fusion.Addons.SimpleKCC;
using UnityEngine;
using UnityEngine.InputSystem;

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

    // Replace Animator with NetworkMecanimAnimator
    [SerializeField] private NetworkMecanimAnimator networkAnimator;
    private bool isInputEnabled = true; // 입력 활성화 여부


    private float currentSpeed;

    private TriggerArea currentTriggerArea;  // Reference to the current TriggerArea
    private ButtonController currentButton;

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
        // 입력을 받아와야 합니다.
        if (GetInput(out NetInput input))
        {
            // 입력이 비활성화된 경우에도 PreviousButtons는 업데이트되어야 합니다.
            if (!isInputEnabled)
            {
                if (input.Buttons.WasPressed(PreviousButtons, InputButton.Trigger))
                {
                    HandleTriggerInteraction();
                }

                // 이전 버튼 상태를 항상 기록
                PreviousButtons = input.Buttons;
                return;  // 입력이 비활성화된 경우, 다른 입력 처리를 하지 않음
            }

            // Apply animation parameters only when input is available and it is a forward tick
            if (Runner.IsForward)
            {
                kcc.AddLookRotation(input.LookDelta * lookSensitivity);

                UpdateMovement(input);
                UpdateCamTarget();

                if (input.Buttons.WasPressed(PreviousButtons, InputButton.Interact))
                {
                    HandleInteraction();
                }

                if (input.Buttons.WasPressed(PreviousButtons, InputButton.Trigger))
                {
                    HandleTriggerInteraction();
                    StartRotateObjectRight();
                }

                if (input.Buttons.WasPressed(PreviousButtons, InputButton.Qtrigger))
                {
                    StartRotateObjectLeft();
                }

                UpdateAnimation(input);
            }

            // 항상 PreviousButtons를 업데이트하여 다음 프레임에서 입력 비교 가능
            PreviousButtons = input.Buttons;
        }
    }

    public override void Render()
    {
        if (isInputEnabled) // 입력이 활성화된 경우에만 카메라 타겟 업데이트
        {
            UpdateCamTarget();
        }
    }

    public void StartRotateObjectRight()
    {
        if (currentButton != null)
        {
            currentButton.StartRotateObjectRight();
        }
    }

    public void StartRotateObjectLeft()
    {
        if (currentButton != null)
        {
            currentButton.StartRotateObjectLeft();
        }
    }

    private void HandleInteraction()
    {
        Ray ray = playerCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 5f))
        {
            Debug.Log("Raycast hit: " + hit.transform.name);

            var glassDoor = hit.transform.GetComponent<GlassDoor>();
            if (glassDoor != null)
            {
                glassDoor.ToggleDoor();  // Handle door interaction
            }

            var collectable = hit.transform.GetComponent<Collectable>();
            if (collectable != null)
            {
                collectable.Collect();
            }
        }
    }

    // New method to handle Trigger (E key) interactions
    private void HandleTriggerInteraction()
    {
        if (currentTriggerArea != null)
        {
            if (currentTriggerArea.IsInteracting())
            {
                currentTriggerArea.ExitInteraction();
            }
            else
            {
                currentTriggerArea.EnterInteraction();
            }
        }
        else
        {
            Debug.Log("No TriggerArea in range.");
        }
    }


    
    private void UpdateMovement(NetInput input)
    {
        currentSpeed = input.Buttons.IsSet(InputButton.Run) ? runSpeed : walkSpeed;
        Vector3 worldDirection = kcc.TransformRotation * new Vector3(input.Direction.x, 0f, input.Direction.y);
        float jump = 0f;

        // Detect jump
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

        // Set parameters using the NetworkMecanimAnimator component
        networkAnimator.Animator.SetBool("Walk", isWalking && !isRunning);
        networkAnimator.Animator.SetBool("Run", isRunning);
    }

    private void UpdateCamTarget()
    {
        camTarget.localRotation = Quaternion.Euler(kcc.GetLookRotation().x, 0f, 0f);
    }


    // This method is called by the TriggerArea when the player enters
    public void SetCurrentTriggerArea(TriggerArea triggerArea)
    {
        currentTriggerArea = triggerArea;
    }

    // This method is called by the TriggerArea when the player exits
    public void ClearCurrentTriggerArea()
    {
        currentTriggerArea = null;
    }

    public void SetCurrentButton(ButtonController buttonController)
    {
        currentButton = buttonController;
    }

    public void ClearCurrentButton()
    {
        currentButton = null;
    }

    public void SetInputEnabled(bool enabled)
    {
        isInputEnabled = enabled;
    }
}
