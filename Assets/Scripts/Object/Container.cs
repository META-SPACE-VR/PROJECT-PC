using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    RobotArm robotArm;

    // Start is called before the first frame update
    void Start()
    {
        robotArm = GameObject.FindGameObjectWithTag("Robot Arm").GetComponent<RobotArm>();
    }

    private void OnCollisionEnter(Collision other) {
        // 다른 컨테이너랑 충돌 시 이동 초기화
        if(other.gameObject.CompareTag("Container")) {
            robotArm.ResetMove();
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Wall")) {
            robotArm.ResetMove();
        }
    }
}
