using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// 플레이어 이름을 표시하는 컴포넌트
/// </summary>
public class WorldCanvasNickname : MonoBehaviour
{
    /// <summary>
    /// 이 객체가 항상 메인 카메라를 향하도록 함
    /// 타겟이 있으면 그 타겟을 따라가고, 타겟이 없으면 이 객체를 삭제하여 정리
    /// </summary>
    
    public TMP_Text worldNicknameText; //플레이어 이름 표시

    [HideInInspector] public Transform target; //이 컴포넌트가 따라갈 타겟의 트랜스폼

    public Vector3 offset; //타겟과의 거리 오프셋

    bool destroying = false; //객체가 삭제 중인지 여부 

    private void LateUpdate()
    {
        if (target) //타겟 존재 시 
        {
            transform.position = target.position + offset; //타겟의 위치에 오프셋을 더해 위치 설정
            transform.rotation = Camera.main.transform.rotation; //카메라가 항상 텍스트 바라보게 
        }
        else if (!destroying) //타겟이 없고 삭제 중이지 않으면 
        {
            destroying = true;
            StartCoroutine(WaitAndDestroy());
        }
    }

    private IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(3); //3초간 대기 

        if (target != null && !target.Equals(null)) yield return null; //타겟이 여전히 존재 시 따라감
        else Destroy(gameObject); //타겟이 없으면 이 객체를 삭제
    }
}
