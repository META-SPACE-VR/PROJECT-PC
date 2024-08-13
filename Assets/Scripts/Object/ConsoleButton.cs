using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ConsoleButton : MonoBehaviour
{
    [SerializeField]
    MoveType buttonMoveType;

    GameObject presser;
    bool isPressed;

    [SerializeField]
    Material onMaterial;
    [SerializeField]
    Material offMaterial;

    void Start()
    {
        isPressed = false;
    }

    private void OnTriggerEnter(Collider other) {
        if(!isPressed) {
            presser = other.gameObject;
            isPressed = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        Debug.Log(other.gameObject.name);
        if(other.gameObject == presser) {
            isPressed = false;
        }
    }

    public void ChangeOnMaterial(MoveType moveType) {
        if(moveType == buttonMoveType) {
            GetComponent<MeshRenderer>().material = onMaterial;
        }
    }

    public void ChangeOffMaterial() {
        GetComponent<MeshRenderer>().material = offMaterial;
    }
}
