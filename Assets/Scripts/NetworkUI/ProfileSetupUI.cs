using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProfileSetupUI : MonoBehaviour
{
    // 플레이어의 닉네임을 입력받기 위한 텍스트 입력 필드
    public TMP_InputField nicknameInput;

    // 뒤로 가기 버튼
    public Button confirmButton;

    // 게임 오브젝트가 시작될 때 호출되는 메소드
    private void Start()
    {
        // 입력 필드의 값이 변경될 때 호출될 메소드를 설정합니다.
        // 입력된 값을 클라이언트 정보의 사용자 이름으로 설정합니다.
        nicknameInput.onValueChanged.AddListener(x => ClientInfo.Username = x);

        // 입력 필드의 값이 변경될 때 호출될 또 다른 메소드를 설정합니다.
        // 입력된 값이 비어 있지 않으면 뒤로 가기 버튼을 활성화합니다.
        nicknameInput.onValueChanged.AddListener(x =>
        {
            // 입력 필드가 비어있지 않으면 뒤로 가기 버튼을 클릭 가능하게 설정합니다.
            confirmButton.interactable = !string.IsNullOrEmpty(x);
        });

        // 클라이언트 정보에서 현재 사용자 이름을 입력 필드에 설정합니다.
        nicknameInput.text = ClientInfo.Username;
    }

    // 프로필 설정이 완료되었는지 확인하는 메소드
    public void AssertProfileSetup()
    {
        // 사용자 이름이 비어있으면
        if (string.IsNullOrEmpty(ClientInfo.Username))
            // 현재 UI 화면으로 포커스를 이동합니다.
            UIScreen.Focus(GetComponent<UIScreen>());
    }
}