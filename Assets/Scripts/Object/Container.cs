using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Container : NetworkBehaviour
{
    RobotArm robotArm;

    Quaternion rotationValue;

    // Start is called before the first frame update
    void Start()
    {
        robotArm = GameObject.FindGameObjectWithTag("Robot Arm").GetComponent<RobotArm>();
        rotationValue = transform.localRotation;
    }

    // transform 값 조정
    void TransformCorrention() {
        // 회전값 조정
        transform.localRotation = rotationValue;

        // 위치값 조정
        float ul = robotArm.GetUnitLength();
        transform.localPosition = new((Mathf.RoundToInt(transform.localPosition.x / (ul / 2) + 5f) - 5f) * (ul / 2), transform.localPosition.y, (Mathf.RoundToInt(transform.localPosition.z / (ul / 2) + 5f) - 5f) * (ul / 2));
    }

    private void OnCollisionEnter(Collision other) {
        // 다른 컨테이너 또는 플레이어랑 충돌 시 이동 초기화
        if(other.gameObject.CompareTag("Container") || other.gameObject.CompareTag("Player")) {
            robotArm.ResetMove();
        }
    }

    private void OnTriggerEnter(Collider other) {
        // 퍼즐 경계의 가상의 벽이랑 충돌 시 이동 초기화
        if(other.gameObject.CompareTag("Wall")) {
            robotArm.ResetMove();
        }

        // 바닥에 닿으면 위치 재조정
        if(other.gameObject.CompareTag("Floor")) {
            Debug.Log("컨테이너 착지");
            TransformCorrention();
        }
    }
}
