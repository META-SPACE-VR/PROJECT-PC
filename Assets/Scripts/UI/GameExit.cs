using UnityEngine;
using UnityEngine.UI; // UI 관련 기능을 사용하기 위한 네임스페이스

public class GameExit : MonoBehaviour
{
    public Button gameExitButton; // GameExitButton 오브젝트

    void Start()
    {
        // GameExitButton이 연결되어 있는지 확인
        if (gameExitButton != null)
        {
            // 버튼 클릭 시 호출될 메서드 연결
            gameExitButton.onClick.AddListener(OnGameExitButtonClick);
        }
    }

    // GameExitButton이 클릭되었을 때 호출되는 메서드
    void OnGameExitButtonClick()
    {
        // 애플리케이션 종료
        Application.Quit();

        // 에디터에서 실행 중일 때는 Play Mode를 종료
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
