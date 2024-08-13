using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectInteraction : MonoBehaviour
{
    [SerializeField] private float range;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private TextMeshProUGUI interactionText;

    private Camera mainCamera;
    private Transform targetObject = null; // 집중할 대상 오브젝트
    private bool isOverObject = false; // 마우스가 오브젝트 위에 있는지 여부

    void Start()
    {
        mainCamera = Camera.main;
        interactionText.gameObject.SetActive(false);
    }

    void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range, layerMask))
        {
            if (hit.collider != null && hit.transform != targetObject)
            {
                if (targetObject != null)
                {
                    OnMouseExit();
                }

                targetObject = hit.transform;
                OnMouseEnter();
            }
        }
        else if (targetObject != null)
        {
            OnMouseExit();
        }

        if (isOverObject && Input.GetKeyDown(KeyCode.E))
        {
            FocusOnObject();
        }
    }

    private void OnMouseEnter()
    {
        isOverObject = true;
        interactionText.text = "상세 보기 (E)";
        interactionText.gameObject.SetActive(true);
    }

    private void OnMouseExit()
    {
        isOverObject = false;
        interactionText.gameObject.SetActive(false);
        targetObject = null;
    }

    private void FocusOnObject()
    {
        // 카메라를 targetObject에 집중시키는 로직
        Debug.Log("카메라 뷰를 " + targetObject.gameObject.name + "에 집중합니다.");
        mainCamera.transform.LookAt(targetObject.position);
    }
}
