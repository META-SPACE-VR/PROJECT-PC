// ButtonVR.cs
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ButtonVR : MonoBehaviour
{
    public GameObject button;
    public float rotationDuration = 1.0f;
    public UnityEvent onPress;
    public UnityEvent onRelease;

    private GameObject presser;
    private GameObject wheel1, wheel2, wheel3, wheel4;
    private bool isPressed;

    void Start()
    {
        isPressed = false;

        wheel1 = GameObject.Find("Wheel_FBX1");
        wheel2 = GameObject.Find("Wheel_FBX2");
        wheel3 = GameObject.Find("Wheel_FBX3");
        wheel4 = GameObject.Find("Wheel_FBX4");

        if (wheel1 == null || wheel2 == null || wheel3 == null || wheel4 == null)
        {
            Debug.LogWarning("Wheel_FBX 객체를 찾을 수 없습니다.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Enter: " + other.gameObject.name);
        if (!isPressed && !RotationManager.isRotating)
        {
            button.transform.localPosition = new Vector3(-0.008f, -1.07f, 0.062f);
            presser = other.gameObject;
            isPressed = true;

            if (onPress != null)
            {
                onPress.Invoke();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Trigger Exit: " + other.gameObject.name);
        if (other.gameObject == presser)
        {
            button.transform.localPosition = new Vector3(0, 0, 0);
            if (onRelease != null)
            {
                onRelease.Invoke();
            }
            isPressed = false;
        }
    }

    public void StartRotateObjectRight()
    {
        if (!RotationManager.isRotating)
        {
            StartCoroutine(RotateObject(1));
        }
    }

    public void StartRotateObjectLeft()
    {
        if (!RotationManager.isRotating)
        {
            StartCoroutine(RotateObject(-1));
        }
    }

    private IEnumerator RotateObject(int n)
    {
        RotationManager.isRotating = true;

        Quaternion startRotation1 = wheel1.transform.rotation;
        Quaternion startRotation2 = wheel2.transform.rotation;
        Quaternion startRotation3 = wheel3.transform.rotation;
        Quaternion startRotation4 = wheel4.transform.rotation;

        Quaternion endRotation1 = startRotation1 * Quaternion.Euler(0, 90 * n, 0);
        Quaternion endRotation2 = startRotation2 * Quaternion.Euler(0, -90 * n, 0);
        Quaternion endRotation3 = startRotation3 * Quaternion.Euler(0, -90 * n, 0);
        Quaternion endRotation4 = startRotation4 * Quaternion.Euler(0, 90 * n, 0);

        float elapsedTime = 0f;
        while (elapsedTime < rotationDuration)
        {
            wheel1.transform.rotation = Quaternion.Slerp(startRotation1, endRotation1, elapsedTime / rotationDuration);
            wheel2.transform.rotation = Quaternion.Slerp(startRotation2, endRotation2, elapsedTime / rotationDuration);
            wheel3.transform.rotation = Quaternion.Slerp(startRotation3, endRotation3, elapsedTime / rotationDuration);
            wheel4.transform.rotation = Quaternion.Slerp(startRotation4, endRotation4, elapsedTime / rotationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        wheel1.transform.rotation = endRotation1;
        wheel2.transform.rotation = endRotation2;
        wheel3.transform.rotation = endRotation3;
        wheel4.transform.rotation = endRotation4;

        RotationManager.isRotating = false;
    }
}
