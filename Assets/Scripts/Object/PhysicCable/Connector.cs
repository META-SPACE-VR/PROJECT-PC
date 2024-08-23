using System.Collections;
using UnityEngine;
using NaughtyAttributes;
using Fusion;

namespace HPhysic
{
    [RequireComponent(typeof(Rigidbody))]
    public class Connector : NetworkBehaviour
    {

        public enum ConType { Male, Female }
        public enum CableColor { White, Red, Green, Yellow, Blue, Cyan, Magenta }

        [field: Header("Settings")]

        [field: SerializeField] public ConType ConnectionType { get; private set; } = ConType.Male;
        [field: SerializeField, OnValueChanged(nameof(UpdateConnectorColor))] public CableColor ConnectionColor { get; private set; } = CableColor.White;

        [SerializeField] private bool makeConnectionKinematic = false;
        private bool _wasConnectionKinematic;

        [SerializeField] private bool hideInteractableWhenIsConnected = false;
        [SerializeField] private bool allowConnectDifrentCollor = false;

        [field: SerializeField] public Connector ConnectedTo { get; private set; }
        [SerializeField] public Connector TargetConnector;

        [Header("Object to set")]
        [SerializeField, Required] private Transform connectionPoint;
        [SerializeField] private MeshRenderer collorRenderer;
        [SerializeField] private ParticleSystem sparksParticle;

        private FixedJoint _fixedJoint;
        public Rigidbody Rigidbody { get; private set; }

        public Vector3 ConnectionPosition => connectionPoint ? connectionPoint.position : transform.position;
        public Quaternion ConnectionRotation => connectionPoint ? connectionPoint.rotation : transform.rotation;
        public Quaternion RotationOffset => connectionPoint ? connectionPoint.localRotation : Quaternion.Euler(Vector3.zero);
        public Vector3 ConnectedOutOffset => connectionPoint ? connectionPoint.right : transform.right;

        [Networked] private NetworkBool isConnectedInternal { get; set; }
        public bool IsConnected => isConnectedInternal;
        public bool IsConnectedRight => IsConnected && ConnectionColor == ConnectedTo.ConnectionColor;

        public override void Spawned()
        {
            base.Spawned();
            Rigidbody = GetComponent<Rigidbody>();

            if (ConnectedTo != null)
            {
                Connector t = ConnectedTo;
                ConnectedTo = null;
                Connect(t);
            }
        }

        private void Awake()
        {
            Rigidbody = gameObject.GetComponent<Rigidbody>();
        }

        private void OnDisable() => Disconnect();

        public void SetAsConnectedTo(Connector secondConnector)
        {
            ConnectedTo = secondConnector;
            _wasConnectionKinematic = secondConnector.Rigidbody.isKinematic;
            UpdateInteractableWhenIsConnected();
        }

        public void Connect(Connector secondConnector)
        {
            if (secondConnector == null)
            {
                Debug.LogWarning("Attempt to connect null");
                return;
            }

            if (IsConnected)
                Disconnect(secondConnector);

            secondConnector.transform.rotation = ConnectionRotation * secondConnector.RotationOffset;
            secondConnector.transform.position = ConnectionPosition - (secondConnector.ConnectionPosition - secondConnector.transform.position);

            _fixedJoint = gameObject.AddComponent<FixedJoint>();
            _fixedJoint.connectedBody = secondConnector.Rigidbody;

            secondConnector.SetAsConnectedTo(this);
            _wasConnectionKinematic = secondConnector.Rigidbody.isKinematic;
            if (makeConnectionKinematic)
                secondConnector.Rigidbody.isKinematic = true;
            ConnectedTo = secondConnector;

            // 스파크 효과
            if (incorrectSparksC == null && sparksParticle && !AreConnected(this, TargetConnector))
            {
                incorrectSparksC = IncorrectSparks();
                StartCoroutine(incorrectSparksC);
            }

            // 연결 시 아웃라인 업데이트
            UpdateInteractableWhenIsConnected();
        }

        public void Disconnect(Connector onlyThis = null)
        {
            if (ConnectedTo == null || onlyThis != null && onlyThis != ConnectedTo)
                return;

            Destroy(_fixedJoint);

            Connector toDisconect = ConnectedTo;
            ConnectedTo = null;
            if (makeConnectionKinematic)
                toDisconect.Rigidbody.isKinematic = _wasConnectionKinematic;
            toDisconect.Disconnect(this);

            // 스파크 효과 정지
            if (sparksParticle)
            {
                sparksParticle.Stop();
                sparksParticle.Clear();
            }

            // 연결 시 아웃라인 업데이트
            UpdateInteractableWhenIsConnected();
        }

        private void UpdateInteractableWhenIsConnected()
        {
            if (hideInteractableWhenIsConnected)
            {
                if (TryGetComponent(out Collider collider))
                    collider.enabled = !IsConnected;
            }
        }

        private IEnumerator incorrectSparksC;
        private IEnumerator IncorrectSparks()
        {
            while (incorrectSparksC != null && sparksParticle && !AreConnected(this, TargetConnector))
            {
                sparksParticle.Play();

                yield return new WaitForSeconds(Random.Range(0.6f, 0.8f));
            }
            incorrectSparksC = null;
        }

        private void UpdateConnectorColor()
        {
            if (collorRenderer == null)
                return;

            Color color = MaterialColor(ConnectionColor);
            MaterialPropertyBlock probs = new();
            collorRenderer.GetPropertyBlock(probs);
            probs.SetColor("_Color", color);
            collorRenderer.SetPropertyBlock(probs);
        }

        private Color MaterialColor(CableColor cableColor) => cableColor switch
        {
            CableColor.White => Color.white,
            CableColor.Red => Color.red,
            CableColor.Green => Color.green,
            CableColor.Yellow => Color.yellow,
            CableColor.Blue => Color.blue,
            CableColor.Cyan => Color.cyan,
            CableColor.Magenta => Color.magenta,
            _ => Color.clear
        };

        public bool CanConnect(Connector secondConnector)
        {
            if (this == secondConnector || IsConnected || secondConnector.IsConnected)
                return false;

            if (ConnectionType == secondConnector.ConnectionType)
                return false;

            return true;
        }

        public bool IsConnectedTo(Connector target)
        {
            if (target == null)
                return false;

            if (ConnectedTo == target)
                return true;

            Connector nextConnector = ConnectedTo;
            while (nextConnector != null)
            {
                if (nextConnector == target)
                {
                    Debug.Log("connected " + nextConnector.name);
                    return true;
                }

                if (nextConnector.name == "Start")
                {
                    Transform parentConnector = nextConnector.transform.parent;
                    Connector endConnector = parentConnector.transform.Find("End").GetComponent<Connector>();
                    if (endConnector != null)
                    {
                        nextConnector = endConnector.ConnectedTo;
                        continue;
                    }
                }
                else if (nextConnector.name == "End")
                {
                    Transform parentConnector = nextConnector.transform.parent;
                    Connector startConnector = parentConnector.transform.Find("Start").GetComponent<Connector>();
                    if (startConnector != null)
                    {
                        nextConnector = startConnector.ConnectedTo;
                        continue;
                    }
                }

                nextConnector = null;
            }

            return false;
        }

        public static bool AreConnected(Connector start, Connector end)
        {
            if (start == null || end == null)
                return false;

            return start.IsConnectedTo(end);
        }
    }
}
