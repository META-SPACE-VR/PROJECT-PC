using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;  // Canvas Text 관리를 위해 필요
using Fusion;

public class ButtonController : MonoBehaviour
{ 
    private Player playerController;

    public GameObject LeftButton;
    public GameObject RightButton;
    public float rotationDuration = 1.0f;
    public Canvas canvas;

    private GameObject wheel1, wheel2, wheel3, wheel4;
    private bool isInteracting = false;

    void Start()
    {

        wheel1 = GameObject.Find("Wheel_FBX1");
        wheel2 = GameObject.Find("Wheel_FBX2");
        wheel3 = GameObject.Find("Wheel_FBX3");
        wheel4 = GameObject.Find("Wheel_FBX4");

        if (wheel1 == null || wheel2 == null || wheel3 == null || wheel4 == null)
        {
            Debug.LogWarning("Wheel_FBX 객체를 찾을 수 없습니다.");
        }

        if(canvas==null){
            Debug.LogWarning("canvas 객체를 찾을 수 없습니다.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("플레이어 발견!");
            canvas.gameObject.SetActive(true);
            isInteracting = true;
            playerController = other.GetComponent<Player>();
            playerController.SetCurrentButton(this);
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && playerController != null && other.gameObject == playerController.gameObject)
        {
            Debug.Log("플레이어 나감!");
            canvas.gameObject.SetActive(false);
            isInteracting = false;
            playerController.ClearCurrentButton();
            
        }
    }

    public void StartRotateObjectRight()
    {
        if (!RotationManager.isRotating && isInteracting)
        {
            StartCoroutine(AnimateButtonPress(RightButton, new Vector3(-0.008f, -1.07f, 0.062f), new Vector3(0, 0, 0)));
            StartCoroutine(RotateObject(1));
        }
    }

    public void StartRotateObjectLeft()
    {
        if (!RotationManager.isRotating && isInteracting)
        {
            StartCoroutine(AnimateButtonPress(LeftButton, new Vector3(-0.008f, -1.07f, 0.062f), new Vector3(0, 0, 0)));
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

    private IEnumerator AnimateButtonPress(GameObject button, Vector3 pressedPosition, Vector3 releasedPosition)
    {
        float animationDuration = 0.2f; // 버튼 이동 애니메이션 지속 시간
        float elapsedTime = 0f;

        // 버튼이 눌리는 애니메이션
        while (elapsedTime < animationDuration)
        {
            button.transform.localPosition = Vector3.Lerp(button.transform.localPosition, pressedPosition, elapsedTime / animationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        button.transform.localPosition = pressedPosition;

        // 버튼이 원래 위치로 돌아가는 애니메이션
        elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            button.transform.localPosition = Vector3.Lerp(button.transform.localPosition, releasedPosition, elapsedTime / animationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        button.transform.localPosition = releasedPosition;
    }

    public bool IsInteracting()
    {
        return isInteracting;
    }

    public Player GetInteractingPlayer() {
        return isInteracting ? playerController : null;
    }
}
