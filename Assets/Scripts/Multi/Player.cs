using Fusion;
using Fusion.Addons.SimpleKCC;
using UnityEngine;
using UnityEngine.InputSystem;
using NaughtyAttributes;
using HInteractions;
using System;
using HPlayer;
using UnityEngine.SceneManagement;

public class Player : NetworkBehaviour, IObjectHolder
{
    public static Player Local;
    private InputManager inputManager;
    [Networked] public string Name { get; private set; }
    
    [SerializeField] private MeshRenderer[] modelParts;
    [SerializeField] private SimpleKCC kcc;
    [SerializeField] private Transform camTarget;
    [SerializeField] private float lookSensitivity = 0.15f;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float jumpImpulse = 10f;

    [Networked] public NetworkString<_16> currentJob { get; set; }

    private NPCInteraction currentNPC;  // 현재 상호작용 중인 NPC
    private WheelchairController currentWheelchair;
    [SerializeField] private Camera playerCamera;

    // Replace Animator with NetworkMecanimAnimator
    [SerializeField] private NetworkMecanimAnimator networkAnimator;
    private bool isInputEnabled = true; // 입력 활성화 여부

    private float currentSpeed;

    private TriggerArea currentTriggerArea;  // Reference to the current TriggerArea
    private Goods currentGoods;  // Reference to the current Goods
    private ButtonController currentButton;

    [Networked] private NetworkButtons PreviousButtons { get; set; }

    [Header("Select")]
    [SerializeField, Required] private Transform playerCameraTransform;
    [SerializeField] private float selectRange = 10f;
    [SerializeField] private LayerMask selectLayer;
    [field: SerializeField, NaughtyAttributes.ReadOnly] public Interactable SelectedObject { get; private set; } = null;

    [Header("Hold")]
    [SerializeField, Required] private Transform handTransform;
    [SerializeField, Min(1)] private float holdingForce = 0.5f;
    [SerializeField] private int heldObjectLayer;
    [SerializeField] [Range(0f, 90f)] private float heldClamXRotation = 45f;
    [field: SerializeField, NaughtyAttributes.ReadOnly] public Liftable HeldObject { get; private set; } = null;

    [field: Header("Input")]
    [field: SerializeField, NaughtyAttributes.ReadOnly] public bool Interacting { get; private set; } = false;

    public event Action OnSelect;
    public event Action OnDeselect;

    public event Action OnInteractionStart;
    public event Action OnInteractionEnd;

    [Header("Inventory")]
    public InventoryManager inventoryManager;
    private bool isZoomed = false;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game")
        {
            inventoryManager = InventoryManager.Instance;
        }
    }

    public override void Spawned()
    {
        kcc.SetGravity(Physics.gravity.y * 2f);

        if (HasInputAuthority)
        {
            Local = this;
            
            foreach (MeshRenderer renderer in modelParts)
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            
            inputManager = Runner.GetComponent<InputManager>();
            inputManager.LocalPlayer = this;
            Name = PlayerPrefs.GetString("Photon.Menu.Username");
            RPC_PlayerName(Name);
            CameraFollow.Instance.SetTarget(camTarget);
            UIManager.Singleton.LocalPlayer = this;
            InventoryManager.Instance.player = this;
            // playerCamera = Camera.main;
            playerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            kcc.Settings.ForcePredictedLookRotation = true;
        }
        DontDestroyOnLoad(this);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
	public void Rpc_SetJob(string job)
	{
		this.currentJob = job;
	}

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_PlayerName(string name)
    {
        Name = name;
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

                if(input.Buttons.WasPressed(PreviousButtons, InputButton.RobotUp)) HandleRobotArmInteraction(MoveType.Up);
                else if(input.Buttons.WasPressed(PreviousButtons, InputButton.RobotDown)) HandleRobotArmInteraction(MoveType.Down);
                else if(input.Buttons.WasPressed(PreviousButtons, InputButton.RobotLeft)) HandleRobotArmInteraction(MoveType.Left);
                else if(input.Buttons.WasPressed(PreviousButtons, InputButton.RobotRight)) HandleRobotArmInteraction(MoveType.Right);
                else if(input.Buttons.WasPressed(PreviousButtons, InputButton.RobotAttach)) HandleRobotArmInteraction(MoveType.Attach);
                if (input.Buttons.WasPressed(PreviousButtons, InputButton.Interact))
                {
                    HandleInteraction();
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

                UpdateInput(input);
                UpdateSelectedObject();
                if (HeldObject)
                    UpdateHeldObjectPosition();

                if (input.Buttons.WasPressed(PreviousButtons, InputButton.Interact))
                {
                    HandleInteraction();
                }

                if (input.Buttons.WasPressed(PreviousButtons, InputButton.Trigger))
                {
                    HandleTriggerInteraction();
                    StartRotateObjectRight();
                    HandleGoodsInteraction();
                }

                if (input.Buttons.WasPressed(PreviousButtons, InputButton.Qtrigger))
                {
                    StartRotateObjectLeft();
                }

                if (input.Buttons.WasPressed(PreviousButtons, InputButton.Transform))
                {
                    HandleTransformInteraction();
                }

                if (input.Buttons.WasPressed(PreviousButtons, InputButton.Zoom))
                {
                    HandleZoomState();
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
            // 유리문 여닫기
            var glassDoor = hit.transform.GetComponent<GlassDoor>();
            if (glassDoor != null)
            {
                glassDoor.ToggleDoor();  // Handle door interaction
            }
            // 믹서 뚜껑 닫기
            var mixerCover = hit.transform.GetComponent<MixerCoverController>();
            if (mixerCover != null)
            {
                mixerCover.ToggleDoor();
            }
            // 액화 장치 뚜껑 닫기
            var liquificationDoor = hit.transform.GetComponent<LiquefactionDoorController>();
            if (liquificationDoor != null)
            {
                liquificationDoor.ToggleDoor();
            }
            // 아이템 줍기
            var collectable = hit.transform.GetComponent<Collectable>();
            if (collectable != null && inventoryManager != null && inventoryManager.pickedItemIndex == -1)
            {
                collectable.Collect();
            }
            // 아이템 두기
            var putable = hit.transform.GetComponent<Putable>();
            if (putable != null && inventoryManager != null && inventoryManager.pickedItemIndex != -1)
            {
                putable.PutItem();
            }
            // 아이템 버리기
            if (putable == null && inventoryManager != null && inventoryManager.pickedItemIndex != -1)
            {
                Vector3 dropPosition = transform.position + transform.forward * 2f + Vector3.up * 2.5f;
                inventoryManager.DropItem(inventoryManager.pickedItemIndex, dropPosition);
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
        else if (currentNPC != null)
        {
            if (!currentNPC.isInteracting)
            {
                currentNPC.StartInteraction();
            }
            else
            {
                currentNPC.AdvanceDialogue();
            }
        }
        else
        {
            Debug.Log("No TriggerArea or NPC in range.");
        }
    }

    private void HandleTransformInteraction()
    {
        if (currentWheelchair != null)
        {
            if (currentWheelchair.isInteracting)
            {
                currentWheelchair.ExitInteraction();
            }
            else
            {
                currentWheelchair.EnterInteraction();
            }
        }
        else
        {
            Debug.Log("No wheelchair in range.");
        }
    }

    private void HandleRobotArmInteraction(MoveType moveType) {
        RobotArm robotArm = GameObject.Find("Robot Arm").GetComponent<RobotArm>();
        if(currentTriggerArea == robotArm.GetTriggerArea() && currentTriggerArea.IsInteracting()) {
            robotArm.Move(moveType);
        }
    }

    private void HandleGoodsInteraction() {
        if(currentGoods) {
            currentGoods.TriggerSpawnFood();
        }
    }

    public void SetCurrentGoods(Goods goods) {
        currentGoods = goods;
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


    public void SetCurrentWheelchair(WheelchairController wheelchair)
    {
        currentWheelchair = wheelchair;
    }

    public void ClearCurrentWheelchair()
    {
        currentWheelchair = null;
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

    public void SetCurrentNPC(NPCInteraction npcInteraction)
    {
        currentNPC = npcInteraction;
    }

    // This method is called by the NPC when the player exits its interaction area
    public void ClearCurrentNPC()
    {
        currentNPC = null;
    }

    private void OnEnable()
    {
        OnInteractionStart += ChangeHeldObject;

        PlayerController.OnPlayerEnterPortal += CheckHeldObjectOnTeleport;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        OnInteractionStart -= ChangeHeldObject;

        PlayerController.OnPlayerEnterPortal -= CheckHeldObjectOnTeleport;

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void UpdateInput(NetInput input)
    {
        bool interacting = input.Buttons.IsSet(InputButton.Interact);
        if (interacting != Interacting)
        {
            Interacting = interacting;
            if (interacting)
                OnInteractionStart?.Invoke();
            else
                OnInteractionEnd?.Invoke();
        }
    }

    private void UpdateSelectedObject()
    {
        Interactable foundInteractable = null;

        // 마우스 위치에서 월드 공간으로 레이캐스트 수행
        Ray ray = playerCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, selectRange, selectLayer))
            foundInteractable = hit.collider.GetComponent<Interactable>();

        if (SelectedObject == foundInteractable)
            return;

        if (SelectedObject)
        {
            SelectedObject.Deselect();
            OnDeselect?.Invoke();
        }

        SelectedObject = foundInteractable;

        if (foundInteractable && foundInteractable.enabled)
        {
            foundInteractable.Select();
            OnSelect?.Invoke();
        }
    }

    private void UpdateHeldObjectPosition()
    {
        HeldObject.Rigidbody.velocity = (handTransform.position - HeldObject.transform.position) * holdingForce;

        Vector3 handRot = handTransform.rotation.eulerAngles;
        if (handRot.x > 180f)
            handRot.x -= 360f;
        handRot.x = Mathf.Clamp(handRot.x, -heldClamXRotation, heldClamXRotation);
        HeldObject.transform.rotation = Quaternion.Euler(handRot + HeldObject.LiftDirectionOffset);
    }
    private void ChangeHeldObject()
    {
        if (HeldObject)
            DropObject(HeldObject);
        else if (SelectedObject is Liftable liftable)
            PickUpObject(liftable);
    }
    private void PickUpObject(Liftable obj)
    {
        if (obj == null)
        {
            Debug.LogWarning($"{nameof(PlayerInteractions)}: Attempted to pick up null object!");
            return;
        }

        HeldObject = obj;
        obj.PickUp(this, heldObjectLayer);
    }
    private void DropObject(Liftable obj)
    {
        if (obj == null)
        {
            Debug.LogWarning($"{nameof(PlayerInteractions)}: Attempted to drop null object!");
            return;
        }

        HeldObject = null;
        obj.Drop();
    }

    private void CheckHeldObjectOnTeleport()
    {
        if (HeldObject != null)
            DropObject(HeldObject);
    }

    private void HandleZoomState()
    {
        if (inventoryManager != null && inventoryManager.pickedItemIndex != -1)
        {
            isZoomed = !isZoomed;
            if (isZoomed)
            {
                inventoryManager.ZoomItem();
            }
            else
            {
                inventoryManager.UnzoomItem();
            }
        }
    }
}
