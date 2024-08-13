using UnityEngine;

public class Rift : MonoBehaviour
{
    public Animator anim;
    public Transform lift;
    public Transform floor2Position; // 2층 위치를 나타내는 Transform
    public Transform floor1Position; // 1층 위치를 나타내는 Transform

    // 플레이어가 리프트에 있는지 여부를 저장할 변수
    private bool isPlayerOnLift = false;

    // 리프트의 현재 층 상태를 저장할 변수
    private bool isOnFloor2 = false;

    void Update()
    {
        // 리프트와 2층 사이의 거리 계산
        float distanceToFloor2 = Vector3.Distance(transform.position, floor2Position.position);
        // 리프트와 1층 사이의 거리 계산
        float distanceToFloor1 = Vector3.Distance(transform.position, floor1Position.position);

        // 2층에서 플레이어가 리프트에 탑승하면
        if (distanceToFloor2 <= 10 && isPlayerOnLift)
        {
            isOnFloor2 = true;
        }
        // 1층에서 플레이어가 리프트에 탑승하면
        else if (distanceToFloor1 <= 10 && isPlayerOnLift)
        {
            isOnFloor2 = false;
        }

        // 애니메이션 상태 업데이트
        UpdateRiftAnimation();
    }

    // 플레이어가 리프트 트리거에 진입할 때 호출됨
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 플레이어가 리프트에 올라탔음을 설정
            isPlayerOnLift = true;
            // 애니메이션 상태 업데이트
            UpdateRiftAnimation();
        }
    }

    // 플레이어가 리프트 트리거에서 나갈 때 호출됨
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 플레이어가 리프트에서 내렸음을 설정
            isPlayerOnLift = false;
            // 애니메이션 상태 업데이트
            UpdateRiftAnimation();
        }
    }

    // 애니메이션 상태를 업데이트하는 함수
    private void UpdateRiftAnimation()
    {
        if (isPlayerOnLift)
        {
            // 플레이어가 리프트에 타고 있을 때, 현재 층에 따라 애니메이션 상태 변경
            if (isOnFloor2)
            {
                anim.SetBool("RiftDown", true); // 2층에서 리프트 내려가는 애니메이션
                anim.SetBool("RiftUp", false);
            }
            else
            {
                anim.SetBool("RiftUp", true); // 1층에서 리프트 올라가는 애니메이션
                anim.SetBool("RiftDown", false);
            }
        }
        else
        {
            // 플레이어가 리프트에 타고 있지 않을 때, 애니메이션 상태 초기화
            anim.SetBool("RiftUp", false);
            anim.SetBool("RiftDown", false);
        }
    }
}
