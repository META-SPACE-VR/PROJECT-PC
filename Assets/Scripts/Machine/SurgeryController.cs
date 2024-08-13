using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SurgeryController : MonoBehaviour
{
    public Button SuctionBtn;
    public Button BleedStopBtn;
    public Button CutBtn; // 새 버튼 추가
    public Animator machineAnimator; // Animator 컴포넌트를 참조합니다.
    public Camera surgeryCamera; // 카메라 참조
    public GameObject[] bloodObjects; // 혈액 오브젝트 배열
    public GameObject bandageObject; // 붕대 오브젝트 참조
    public TriggerArea triggerArea; // TriggerArea 참조 추가

    private int currentBloodIndex = 0; // 현재 비활성화할 혈액 오브젝트의 인덱스

    void Start()
    {
        // 버튼 클릭 이벤트 설정
        SuctionBtn.onClick.AddListener(OnSuctionButtonClicked);
        BleedStopBtn.onClick.AddListener(OnBleedStopButtonClicked);
        CutBtn.onClick.AddListener(OnCutButtonClicked); // 새 버튼 클릭 이벤트 설정

        BleedStopBtn.gameObject.SetActive(false); // 처음에 BleedStopBtn 숨기기
    }

    void OnSuctionButtonClicked()
    {
        // Center_Suck 트리거 활성화
        Debug.Log("흡입 버튼 클릭됨");
        machineAnimator.SetTrigger("Center_Suck");

        // 혈액 오브젝트를 하나씩 비활성화
        if (currentBloodIndex < bloodObjects.Length)
        {
            bloodObjects[currentBloodIndex].SetActive(false);
            currentBloodIndex++;

            // 모든 혈액 오브젝트가 비활성화되었으면 BleedStopBtn 버튼 활성화
            if (currentBloodIndex == bloodObjects.Length)
            {
                BleedStopBtn.gameObject.SetActive(true);
            }
        }
        else
        {
            Debug.Log("더 이상 비활성화할 혈액 오브젝트가 없습니다.");
        }
    }

    void OnBleedStopButtonClicked()
    {
        // Right_BleedStop 트리거 활성화
        machineAnimator.SetTrigger("Right_BleedStop");

        // 붕대 오브젝트 활성화
        bandageObject.SetActive(true);

        // 카메라를 왼쪽으로 0.03만큼 이동
        Vector3 newPosition = surgeryCamera.transform.position;
        newPosition.x -= 0.03f;
        surgeryCamera.transform.position = newPosition;

        // BleedStopBtn 버튼 비활성화
        BleedStopBtn.gameObject.SetActive(false);

        // TriggerArea의 ExitInteraction 호출
        if (triggerArea != null)
        {
            triggerArea.ExitInteraction(); // 상호작용 종료
        }
    }

    void OnCutButtonClicked()
    {
        // 카메라를 오른쪽으로 0.03만큼 이동
        Vector3 newPosition = surgeryCamera.transform.position;
        newPosition.x += 0.03f;
        surgeryCamera.transform.position = newPosition;

        // CutBtn 버튼 비활성화
        CutBtn.gameObject.SetActive(false);
    }
}
