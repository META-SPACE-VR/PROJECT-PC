using UnityEngine;

public class AlarmController : MonoBehaviour
{
    public Light alarmLight; // 반짝이는 조명
    public AudioSource alarmSound; // 경보음 오디오 소스
    public float flashSpeed = 5f; // 반짝이는 속도 증가
    public float flashIntensity = 2f; // 조명 강도 증가

    private bool isAlarming = false;
    private float alarmDuration = 20f; // 알람 지속 시간 20초

    void Start()
    {
        StartAlarm();
    }

    void Update()
    {
        if (isAlarming)
        {
            alarmLight.intensity = Mathf.PingPong(Time.time * flashSpeed, flashIntensity);
            Debug.Log("Current Intensity: " + alarmLight.intensity);
        }
    }

    public void StartAlarm()
    {
        isAlarming = true;
        if (alarmSound != null)
        {
            alarmSound.Play(); // 경보음 재생 시작
            Invoke("StopAlarm", alarmDuration); // 20초 후 알람 중지
        }
    }

    public void StopAlarm()
    {
        isAlarming = false;
        if (alarmSound != null)
        {
            alarmSound.Stop(); // 경보음 재생 중지
        }
        alarmLight.intensity = 0; // 조명 꺼짐
    }
}
