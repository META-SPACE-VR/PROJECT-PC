using System.Collections;
using UnityEngine;
using NaughtyAttributes;

namespace HPhysic
{
    [RequireComponent(typeof(Rigidbody))]
    public class Connector : MonoBehaviour
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

        public bool IsConnected => ConnectedTo != null;
        public bool IsConnectedRight => IsConnected && ConnectionColor == ConnectedTo.ConnectionColor;



        private void Awake()
        {
            Rigidbody = gameObject.GetComponent<Rigidbody>();
        }

        private void Start()
        {
            UpdateConnectorColor();

            if (ConnectedTo != null)
            {
                Connector t = ConnectedTo;
                ConnectedTo = null;
                Connect(t);
            }
            
            // if(TargetConnector != null){
            //     IsConnectedTo(TargetConnector);
            // }
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

            // 중요한 부분: 재귀 방지
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
            // while (incorrectSparksC != null && sparksParticle)
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
            // 동일한 객체나 이미 연결된 커넥터는 연결 불가
            if (this == secondConnector || IsConnected || secondConnector.IsConnected)
                return false;

            // F-F, M-M 연결 금지, F-M만 연결 가능
            if (ConnectionType == secondConnector.ConnectionType)
                return false;

            // 색상이 다른 경우 연결 가능 여부 확인
            // return allowConnectDifrentCollor || secondConnector.allowConnectDifrentCollor || ConnectionColor == secondConnector.ConnectionColor;
            return true;
        }

        // 커넥터가 다른 커넥터에 연결되어 있는지 확인
        public bool IsConnectedTo(Connector target)
        {
            if (target == null)
                return false;

            if (ConnectedTo == target)
                return true;

            Connector nextConnector = ConnectedTo;
            while (nextConnector != null)
            {
                // Debug.Log("2: " + nextConnector.name);
                if (nextConnector == target)
                {
                    Debug.Log("connected " + nextConnector.name);
                    return true;
                }

                // `Start` 
                if (nextConnector.name == "Start")
                {
                    // Debug.Log("start잇음");
                    Transform parentConnector = nextConnector.transform.parent;
                    Connector endConnector = parentConnector.transform.Find("End").GetComponent<Connector>();
                    if (endConnector != null)
                    {
                        // Debug.Log("4-0: " + endConnector.name);
                        nextConnector = endConnector.ConnectedTo;
                        // Debug.Log("4-1: " + (nextConnector != null ? nextConnector.name : "null"));
                        continue; // 다음 루프로 이동
                    } 
                }

                // `End`
                else if (nextConnector.name == "End")
                {
                    // Debug.Log("end잇음");
                    Transform parentConnector = nextConnector.transform.parent;
                    Connector startConnector = parentConnector.transform.Find("Start").GetComponent<Connector>();
                    if (startConnector != null)
                    {
                        // Debug.Log("4-2: " + startConnector.name);
                        nextConnector = startConnector.ConnectedTo;
                        // Debug.Log("4-3: " + (nextConnector != null ? nextConnector.name : "null"));
                        continue; // 다음 루프로 이동
                    } 
                }

                // 더 이상 연결이 없는 경우
                // Debug.Log("5: null");
                nextConnector = null;
            }

            // Debug.Log("6: null");
            return false;
        }


        // 두 커넥터가 연결되어 있는지 확인
        public static bool AreConnected(Connector start, Connector end)
        {
            if (start == null || end == null)
                return false;

            return start.IsConnectedTo(end);
        }
    }
}