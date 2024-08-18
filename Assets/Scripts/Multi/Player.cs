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

    private bool isInputEnabled = true; // Default to true
    private float currentSpeed;

    private TriggerArea currentTriggerArea;  // Reference to the current TriggerArea

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
        // Apply animation parameters only when input is available and it is a forward tick
        if (GetInput(out NetInput input) && Runner.IsForward)
        {
            // Rotate based on input
            kcc.AddLookRotation(input.LookDelta * lookSensitivity);

            // Update movement
            UpdateMovement(input);

            // Update camera target
            UpdateCamTarget();

            // Handle interaction for mouse click
            if (input.Buttons.WasPressed(PreviousButtons, InputButton.Interact))
            {
                HandleInteraction(); // Example: Handle object interaction
            }

            // Handle interaction for E key (Trigger)
            if (input.Buttons.WasPressed(PreviousButtons, InputButton.Trigger))
            {
                HandleTriggerInteraction(); // Example: Handle trigger interaction (E key)
            }

            // Update animation parameters based on input
            UpdateAnimation(input);

            PreviousButtons = input.Buttons;
        }
    }

    public override void Render()
    {
        UpdateCamTarget();
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

    public void SetInputEnabled(bool enabled)
    {
        isInputEnabled = enabled;
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
}
