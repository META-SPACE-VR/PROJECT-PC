using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GameSuccessController : MonoBehaviour
{
    public GameObject mainCanvas;
    public Timer timer;  // Timer 스크립트 참조
    public GameObject gameSuccessCanvas; // Game Success Canvas 오브젝트
    public TextMeshProUGUI timeText; // 탈출까지 걸린 시간을 표시할 TextMeshProUGUI 오브젝트
    public List<EscapePoint> escapePoints; // 탈출 정 위치들
    public List<Putable> putables; // Putable 오브젝트들
    public List<string> requiredItems; // 필요한 아이템 오브젝트들
    private bool isGameSuccess = false;

    void Start()
    {
        // 초기 상태에서는 게임 성공 캔버스를 비활성화
        gameSuccessCanvas.SetActive(false);
    }

    void Update()
    {
        CheckEscapeConditions();
    }

    private void CheckEscapeConditions()
    {
        // 탈출 정 위치에 있는 플레이어 수를 초기화
        int playersInEscapePoints = 0;

        // 모든 탈출 정 위치에 있는 플레이어 수를 합산
        foreach (EscapePoint point in escapePoints)
        {
            playersInEscapePoints += point.playersInPoint;
        }

        // 모든 플레이어가 탈출 정 위치에 있는지 확인
        if (playersInEscapePoints == 1 && AreAllItemsPlacedCorrectly() && !isGameSuccess)
        {
            isGameSuccess = true;

            // 타이머 멈추기
            timer.StopTimer();

            // 경과 시간을 문자열로 가져와서 UI 텍스트에 설정
            timeText.text = "클리어 시간: " + timer.GetClearTimeString();

            gameSuccessCanvas.SetActive(true); // 게임 성공 캔버스 활성화
            mainCanvas.SetActive(false); // 메인 캔버스 비활성화
        }
    }

    // 모든 필요한 아이템이 올바른 위치에 놓였는지 확인
    private bool AreAllItemsPlacedCorrectly()
    {
        foreach(string itemName in requiredItems) {
            bool flag = false;

            foreach (Putable putable in putables)
            {
                if (putable.putItem != null && putable.putItem.gameObject.GetComponent<Collectable>().Name == itemName)
                {
                    flag = true;
                    break;
                }
            }

            if(!flag) return false;
        }
        
        return true;
    }
}
