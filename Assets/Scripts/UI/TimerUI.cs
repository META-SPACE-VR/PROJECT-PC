using UnityEngine;
using TMPro;

public class TimerTMP : MonoBehaviour
{
    public TextMeshProUGUI timerText; // TMP Text 요소를 할당받기 위한 변수

    [SerializeField]
    Timer timer;

    // 게임이 시작될 때 호출되는 메서드
    void Start()
    {
        timerText.text = "00:00:00.000";
    }

    // 매 프레임마다 호출되는 메서드
    void Update()
    {
        timerText.text = timer.GetTimeString();
    }
}
