using UnityEngine;
using TMPro;
using System.Collections;

public class EmergencyTextController : MonoBehaviour
{
    public TextMeshProUGUI emergencyText; // TextMeshProUGUI 컴포넌트
    public float displayTime = 20f; // 전체 텍스트 표시 시간

    void Start()
    {
        StartCoroutine(DisplayEmergencyText());
    }

    IEnumerator DisplayEmergencyText()
    {
        string[] messages = new string[]
        {
            "비상 대피 상황입니다! 거대한 운석과의 충돌로 우주선이 심각한 손상을 입었습니다.",
            "즉시 대피 준비를 하십시오.",
            "모든 연구원은 신분 확인 카드를 챙기고, 즉시 로비로 이동하십시오.",
            "로비에서 비상 탈출 절차가 곧 시작됩니다.",
            "지체할 시간이 없습니다! 신분 확인 카드를 반드시 챙기고, 지금 바로 로비로 이동하십시오!"
        };

        float delayPerMessage = displayTime / messages.Length;

        for (int i = 0; i < messages.Length; i++)
        {
            emergencyText.text = messages[i];
            yield return new WaitForSeconds(delayPerMessage);
        }

        emergencyText.text = ""; // 모든 메시지가 표시된 후 텍스트 지우기 (선택 사항)
    }
}
