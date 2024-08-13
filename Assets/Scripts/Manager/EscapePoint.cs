using UnityEngine;

public class EscapePoint : MonoBehaviour
{
    public int playersInPoint = 0; // 탈출 정 위치에 있는 플레이어 수

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playersInPoint++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playersInPoint--;
        }
    }
}
