using System;
using UnityEngine;
using UnityEngine.UI;

public class LobbyItemUI : MonoBehaviour {

    // 화면에 표시할 텍스트와 이미지들을 여기에 연결해줍니다.
    public Text username; // 플레이어의 이름을 표시할 텍스트
    public Image ready; // 플레이어가 준비되었는지 여부를 나타내는 이미지
    public Image leader; // 플레이어가 리더인지 여부를 나타내는 이미지

    private RoomPlayer _player; // 현재 플레이어의 정보를 저장할 변수

    // 이 함수는 플레이어 정보를 설정하는 함수입니다.
    public void SetPlayer(RoomPlayer player) {
        _player = player; // 인자로 받은 플레이어 정보를 저장합니다.
    }

    // 매 프레임마다 호출되는 함수입니다. 게임의 화면이 계속 업데이트되기 때문에 여기서 정보를 갱신합니다.
    private void Update() {
        // 플레이어 정보가 유효한지 확인합니다.
        if (_player.Object != null && _player.Object.IsValid)
        {
            // 플레이어의 이름을 화면에 표시합니다.
            username.text = _player.Username.Value;
            
            // 플레이어가 준비되었는지 여부에 따라 이미지를 표시합니다.
            ready.gameObject.SetActive(_player.IsReady);
        }
    }
}