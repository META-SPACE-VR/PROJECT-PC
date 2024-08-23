using UnityEngine;

public class Timer : MonoBehaviour
{
    public float limitTime = 60f; // 제한 시간 (초)
    public GameObject mainCanvas;
    public GameObject gameOverCanvas; // GameOverCanvas 오브젝트

    private float elapsedTime = 0f; // 경과 시간
    private bool isGameOver = false; // 게임 오버 상태

    // 게임이 시작될 때 호출되는 메서드
    void Start()
    {
        elapsedTime = 0f;
        isGameOver = false;
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(false); // 게임 오버 캔버스 숨기기
        }
    }

    // 매 프레임마다 호출되는 메서드
    void Update()
    {
        if (!isGameOver)
        {
            // 경과 시간 업데이트
            elapsedTime += Time.deltaTime;

            // 제한 시간 초과 체크
            if (elapsedTime >= limitTime)
            {
                elapsedTime = limitTime; // 시간을 제한 시간으로 설정 (초과 방지)
                GameOver(); // 게임 오버 처리
            }
        }
    }

    // 게임 오버 처리 메서드
    void GameOver()
    {
        isGameOver = true;
        if (gameOverCanvas != null)
        {
            mainCanvas.SetActive(false); // 메인 캔버스 비활성화
            gameOverCanvas.SetActive(true); // 게임 오버 캔버스 표시
        }
    }

    // 타이머 멈추기 메서드
    public void StopTimer()
    {
        isGameOver = true;
    }

    // 제한 시간에 대한 남은 시간을 문자열로 반환
    public string GetTimeString()
    {
        float remainingTime = Mathf.Max(0f, limitTime - elapsedTime);

        int hours = Mathf.FloorToInt(remainingTime / 3600f);
        int minutes = Mathf.FloorToInt((remainingTime % 3600) / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        int miliSeconds = Mathf.FloorToInt((remainingTime % 1f) * 1000f);

        // 시간 형식으로 문자열 생성 후 리턴
        return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, miliSeconds);
    }

    public string GetClearTimeString(){
        int hours = Mathf.FloorToInt(elapsedTime / 3600f);
        int minutes = Mathf.FloorToInt((elapsedTime % 3600) / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        int miliSeconds = Mathf.FloorToInt((elapsedTime % 1f) * 1000f);

        // 시간 형식으로 문자열 생성 후 리턴
        return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, miliSeconds);
    }
}
