using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class MoveEvent : UnityEvent<MoveType> {}

public class RobotArm : MonoBehaviour
{
    [SerializeField]
    Vector2 initPosInPuzzle; // 로봇 팔의 초기 위치 (퍼즐판 기준)

    Vector2 curPosInPuzzle; // 로봇 팔의 현재 위치 (퍼즐판 기준)
    Vector2 targetPosInPuzzle; // 로봇팔의 목표 위치 (퍼즐판 기준)
    Vector3 offset1 = Vector3.zero; // 보정 값 1
    Vector3 offset2 = Vector3.zero; // 보정 값 2

    [SerializeField]
    float unitLength; // 한칸의 단위 길이

    [SerializeField]
    float moveTime; // 한칸 움직이는데 걸리는 시간

    float movePercentage; // 움직인 비율

    bool isMoving = false; // 이동 상태
    bool isMoveReset = false; // 이동 초기화 여부
    bool isAttaching = false; // 컨테이너 부착 여부
    GameObject selectedContainer = null; // 선택된 컨테이너

    [SerializeField]
    Transform attachPoint; // 부착 위치

    [SerializeField]
    AudioSource attachSound; // 컨테이너 부착 소리

    [SerializeField]
    AudioSource detachSound; // 컨테이너 부착 해제 소리

    [SerializeField]
    Transform lightPoint; // 빛 기준점

    [SerializeField]
    Transform containerPuzzle; // 컨테이너 퍼즐

    readonly float epsilon = 0.0001f;

    public MoveEvent moveStarted; // 이동 시작 시 발생하는 이벤트
    public UnityEvent moveFinished; // 이동 종료 시 발생하는 이벤트

    // Start is called before the first frame update
    void Start()
    {
        // 로봇 팔을 초기 위치에 둔다.
        curPosInPuzzle = targetPosInPuzzle = initPosInPuzzle;
        transform.localPosition = new Vector3(unitLength * (initPosInPuzzle.x - 2.5f), transform.localPosition.y, unitLength * (initPosInPuzzle.y - 2.5f));
    }

    // Update is called once per frame
    void Update()
    {   
        // isMoving이 true면 로봇팔 이동 시도
        if(isMoving) {
            Vector3 curPos = new Vector3(unitLength * (curPosInPuzzle.x - 2.5f), transform.localPosition.y, unitLength * (curPosInPuzzle.y - 2.5f)) + offset1;
            Vector3 newPos = new Vector3(unitLength * (targetPosInPuzzle.x - 2.5f), transform.localPosition.y, unitLength * (targetPosInPuzzle.y - 2.5f)) + offset2;
            UpdatePosition(curPos, newPos, isMoveReset); // 로봇팔의 위치 설정
        }
    }

    public float GetUnitLength() { return unitLength; }

    // 현재 위치와 목표 위치에 따라 새로운 위치 계산 후 그 위치로 설정
    void UpdatePosition(Vector3 curPos, Vector3 newPos, bool isBackWard) {
        // 다음 movePercentage 계산
        movePercentage += Time.deltaTime/moveTime * (isBackWard ? -1 : 1);
        movePercentage = Mathf.Min(1, movePercentage);
        movePercentage = Mathf.Max(0, movePercentage);
        
        // 새로운 위치 반영
        transform.localPosition = Vector3.Lerp(curPos, newPos, movePercentage);

        // 이동이 종료되면 수행하는 동작
        if(movePercentage == 0 || movePercentage == 1) {
            if(movePercentage == 1) { // 이동이 제대로 종료되면
                curPosInPuzzle = targetPosInPuzzle;
                offset1 = offset2;
                
                if(selectedContainer && !isAttaching) {
                    AttachContainer(selectedContainer);
                }
            }
            else { // 이동 초기화가 완료되면
                targetPosInPuzzle = curPosInPuzzle;
            }

            movePercentage = 0;
            isMoving = false;
            isMoveReset = false;
            moveFinished.Invoke();
        }
    }

    // 이동 관련 입력 확인
    void Move(MoveType moveType) {
        if(isMoving) return;

        targetPosInPuzzle = curPosInPuzzle;

        switch(moveType) {
            case MoveType.Left:
                targetPosInPuzzle.x -= 1;
                break;
            case MoveType.Right:
                targetPosInPuzzle.x += 1;
                break;
            case MoveType.Up:
                targetPosInPuzzle.y += 1;
                break;
            case MoveType.Down:
                targetPosInPuzzle.y -= 1;
                break;
            case MoveType.Attach:
                if(isAttaching) {
                    DetachContainer();
                    offset2 = Vector3.zero;
                    selectedContainer = null;
                }
                else {
                    if(Physics.Raycast(lightPoint.position, -1 * transform.up, out RaycastHit hit)) {
                        if(hit.collider.CompareTag("Container")) {
                            selectedContainer = hit.collider.gameObject;
                            
                            offset2 = hit.collider.transform.localPosition - transform.localPosition;
                            offset2.y = 0;

                            if((offset2 - Vector3.zero).magnitude <= epsilon) {
                                AttachContainer(selectedContainer);
                            }
                        }
                    }
                }
                break;
        }

        Vector3 curPos = new Vector3(unitLength * (curPosInPuzzle.x - 2.5f), transform.localPosition.y, unitLength * (curPosInPuzzle.y - 2.5f)) + offset1;
        Vector3 newPos = new Vector3(unitLength * (targetPosInPuzzle.x - 2.5f), transform.localPosition.y, unitLength * (targetPosInPuzzle.y - 2.5f)) + offset2;
        if(curPos != newPos) {
            isMoving = true;
            moveStarted.Invoke(moveType);
        }
    }

    public void MoveLeft() { Move(MoveType.Left); } // 왼쪽으로 이동
    public void MoveRight() { Move(MoveType.Right); } // 오른쪽으로 이동
    public void MoveUp() { Move(MoveType.Up); } // 위쪽으로 이동
    public void MoveDown() { Move(MoveType.Down); } // 아래쪽으로 이동
    public void MoveAttach() { Move(MoveType.Attach); } // 컨테이너 부착을 위한 이동

    // 로봇 팔에 컨테이너 부착
    void AttachContainer(GameObject container) {
        attachSound.Play();

        isAttaching = true;
        container.transform.SetParent(attachPoint);
        container.transform.localPosition = Vector3.zero;
        container.GetComponent<Rigidbody>().useGravity = false;
    }

    // 로봇 팔에서 컨테이너 제거 
    void DetachContainer() {
        detachSound.Play();

        GameObject container = attachPoint.GetChild(0).gameObject;
        
        attachPoint.DetachChildren();
        container.transform.parent = containerPuzzle;
        container.GetComponent<Rigidbody>().useGravity = true;

        selectedContainer = null;
        isAttaching = false;
    }

    // 이동 초기화
    public void ResetMove() {
        isMoveReset = true;
    }

    private void OnTriggerEnter(Collider other) {
        // 퍼즐 경계의 가상의 벽에 닿으면 이동 초기화
        if(other.CompareTag("Wall")) {
            ResetMove();
        }
    }
}
