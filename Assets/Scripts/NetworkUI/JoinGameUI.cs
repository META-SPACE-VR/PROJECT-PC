using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JoinGameUI : MonoBehaviour
{
    public TMP_InputField lobbyName;
    public Button confirmButton;

    // UI가 활성화될 때 호출되는 메소드
    private void OnEnable()
    {
        // 로비 이름을 입력 필드의 텍스트로 설정합니다.
        SetLobbyName(lobbyName.text);
    }

    // 게임 오브젝트가 시작될 때 호출되는 메소드
    private void Start()
    {
        // 입력 필드의 값이 변경될 때 SetLobbyName 메소드를 호출하도록 설정합니다.
        lobbyName.onValueChanged.AddListener(SetLobbyName);

        // 클라이언트 정보에서 로비 이름을 가져와서 입력 필드에 설정합니다.
        lobbyName.text = ClientInfo.LobbyName;
    }

    // 로비 이름을 설정하는 메소드
    private void SetLobbyName(string lobby)
    {
        // 클라이언트 정보에 로비 이름을 저장합니다.
        ClientInfo.LobbyName = lobby;
        // confirmButton.interactable = !string.IsNullOrEmpty(lobby);
    }
}