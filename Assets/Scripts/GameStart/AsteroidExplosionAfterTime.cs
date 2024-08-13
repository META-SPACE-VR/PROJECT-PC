using System.Collections;
using UnityEngine;

public class AsteroidExplosionAfterTime : MonoBehaviour
{
    [SerializeField]
    private AestroidBlow Rock;
    [SerializeField]
    private float delayTime = 5.4f; // 지연 시간 (2초로 변경하여 테스트)

    void Start()
    {
        Debug.Log("Start method called"); // 디버그 로그 추가
        Debug.Log(Rock != null ? "Rock is assigned" : "Rock is not assigned"); // Rock 할당 확인
        StartCoroutine(ExplodeAfterDelay());
    }

    IEnumerator ExplodeAfterDelay()
    {
        Debug.Log("Coroutine started"); // 디버그 로그 추가
        yield return new WaitForSeconds(delayTime); // 지연 시간 대기

        // 돌 폭발
        if (Rock != null)
        {
            Debug.Log("Exploding asteroid..."); // 디버그 로그 추가
            Rock.Explode();
        }
        else
        {
            Debug.LogError("DestructibleRock script is not assigned or not found on the asteroid object.");
        }
    }
}
