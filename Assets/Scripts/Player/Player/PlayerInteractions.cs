using UnityEngine;
using NaughtyAttributes;
using HInteractions;
using System;

namespace HPlayer
{
    public class PlayerInteractions : MonoBehaviour, IObjectHolder
    {
        [Header("Select")]
        [SerializeField, Required] private Transform playerCamera;
        [SerializeField] private float selectRange = 10f;
        [SerializeField] private LayerMask selectLayer;
        [field: SerializeField, ReadOnly] public Interactable SelectedObject { get; private set; } = null;

        [Header("Hold")]
        [SerializeField, Required] private Transform handTransform;
        [SerializeField, Min(1)] private float holdingForce = 0.5f;
        [SerializeField] private int heldObjectLayer;
        [SerializeField] [Range(0f, 90f)] private float heldClamXRotation = 45f;
        [field: SerializeField, ReadOnly] public Liftable HeldObject { get; private set; } = null;

        [field: Header("Input")]
        [field: SerializeField, ReadOnly] public bool Interacting { get; private set; } = false;

        public event Action OnSelect;
        public event Action OnDeselect;

        public event Action OnInteractionStart;
        public event Action OnInteractionEnd;

        private void OnEnable()
        {
            OnInteractionStart += ChangeHeldObject;

            PlayerController.OnPlayerEnterPortal += CheckHeldObjectOnTeleport;
        }

        private void OnDisable()
        {
            OnInteractionStart -= ChangeHeldObject;

            PlayerController.OnPlayerEnterPortal -= CheckHeldObjectOnTeleport;
        }

        private void Update()
        {
            UpdateInput();

            UpdateSelectedObject();

            if (HeldObject)
                UpdateHeldObjectPosition();
        }

        #region -input-

        private void UpdateInput()
        {
            // OVRInput 사용하여 입력 감지
            bool interacting = OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) || OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger);
            if (interacting != Interacting)
            {
                Interacting = interacting;
                if (interacting)
                    OnInteractionStart?.Invoke();
                else
                    OnInteractionEnd?.Invoke();
            }
        }

        #endregion

        #region -selected object-

        private void UpdateSelectedObject()
        {
            Interactable foundInteractable = null;

            // 손의 위치에서 월드 공간으로 레이캐스트 수행
            Ray ray = new Ray(handTransform.position, handTransform.forward);

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

        #endregion

        #region -held object-

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

        #endregion
    }
}
