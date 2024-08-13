using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using Unity.VisualScripting;
using UnityEngine;

public class Cable : MonoBehaviour
{   
    // 전선의 연결부
    [SerializeField]
    Transform attachPoint1;
    [SerializeField]
    Transform attachPoint2;

    [SerializeField]
    float attachRadius;

    // 전선이 연결된 구멍
    public GameObject connectedHole1 { get; private set; }
    public GameObject connectedHole2 { get; private set; }

    Rigidbody rigidBody;

    [SerializeField]
    PanelManager panelManager;

    // Start is called before the first frame update
    void Start()
    {
        connectedHole1 = null;
        connectedHole2 = null;
        rigidBody = GetComponent<Rigidbody>();

        rigidBody.useGravity = true;
        rigidBody.isKinematic = false;

        // Attach Point 주위에 구멍이 있는지 확인
        Collider[] hitCollider1 = Physics.OverlapSphere(attachPoint1.position, attachRadius, 1 << LayerMask.NameToLayer("Wire Hole"));
        foreach (Collider collider in hitCollider1) {
            Debug.Log("Collision!");
            CableHole cableHole = collider.GetComponent<CableHole>();
            if(!cableHole.isConnected) {
                connectedHole1 = collider.gameObject;
                cableHole.isConnected = true;
                break;
            }
        }

        Collider[] hitCollider2 = Physics.OverlapSphere(attachPoint2.position, attachRadius, 1 << LayerMask.NameToLayer("Wire Hole"));
        foreach (Collider collider in hitCollider2) {
            CableHole cableHole = collider.GetComponent<CableHole>();
            if(!cableHole.isConnected) {
                connectedHole2 = collider.gameObject;
                cableHole.isConnected = true;
                break;
            }
        }

        // 구멍에 하나라도 연결 시
        if(connectedHole1 || connectedHole2) {
            rigidBody.useGravity = false;
            rigidBody.isKinematic = true;

            if(connectedHole1) {
                transform.localRotation = Quaternion.identity;

                Vector3 moveVector = connectedHole1.transform.position - attachPoint1.position;
                transform.position += moveVector;
            }
            if(connectedHole2) {
                transform.localRotation = Quaternion.identity;

                Vector3 moveVector = connectedHole2.transform.position - attachPoint2.position;
                transform.position += moveVector;
            }
        }

        // 둘 다 연결 시
        if(connectedHole1 && connectedHole2) {
            panelManager.ConnectPair(connectedHole1, connectedHole2);
        }
    }

    // 케이블을 쥘 때 동작
    public void OnGrab() {
        // 둘 다 연결되어 있던 상태라면
        if(connectedHole1 && connectedHole2) {
            panelManager.DisconnectPair(connectedHole1, connectedHole2);
        }

        // 연결 해제
        if(connectedHole1) { connectedHole1.GetComponent<CableHole>().isConnected = false; connectedHole1 = null; }
        if(connectedHole2) { connectedHole2.GetComponent<CableHole>().isConnected = false; connectedHole2 = null; }

        // 리지드 바디 세팅
        rigidBody.useGravity = true;
        rigidBody.isKinematic = false;
    }

    // 케이블을 놓을 때 동작
    public void OnRelease() {
        // Attach Point 주위에 구멍이 있는지 확인
        Collider[] hitCollider1 = Physics.OverlapSphere(attachPoint1.position, attachRadius, 1 << LayerMask.NameToLayer("Wire Hole"));
        foreach (Collider collider in hitCollider1) {
            CableHole cableHole = collider.GetComponent<CableHole>();
            if(!cableHole.isConnected) {
                connectedHole1 = collider.gameObject;
                cableHole.isConnected = true;
                break;
            }
        }

        Collider[] hitCollider2 = Physics.OverlapSphere(attachPoint2.position, attachRadius, 1 << LayerMask.NameToLayer("Wire Hole"));
        foreach (Collider collider in hitCollider2) {
            CableHole cableHole = collider.GetComponent<CableHole>();
            if(!cableHole.isConnected) {
                connectedHole2 = collider.gameObject;
                cableHole.isConnected = true;
                break;
            }
        }

        // 구멍에 하나라도 연결 시
        if(connectedHole1 || connectedHole2) {
            rigidBody.useGravity = false;
            rigidBody.isKinematic = true;

            if(connectedHole1) {
                transform.localRotation = Quaternion.identity;

                Vector3 moveVector = connectedHole1.transform.position - attachPoint1.position;
                transform.position += moveVector;
            }
            if(connectedHole2) {
                transform.localRotation = Quaternion.identity;

                Vector3 moveVector = connectedHole2.transform.position - attachPoint2.position;
                transform.position += moveVector;
            }
        }

        // 둘 다 연결 시
        if(connectedHole1 && connectedHole2) {
            panelManager.ConnectPair(connectedHole1, connectedHole2);
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attachPoint1.position, attachRadius);
        Gizmos.DrawWireSphere(attachPoint2.position, attachRadius);
    }
}
