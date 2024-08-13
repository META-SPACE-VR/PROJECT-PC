using System.Collections.Generic;
using System.Linq;
using Managers;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyUI : MonoBehaviour, IDisabledUI
{
    public GameObject mainCanvasEventSystem; 

    // UI 요소들을 연결할 변수들
    public GameObject textPrefab; // 플레이어 정보를 표시할 UI 프리팹
    public Transform parent; // UI 프리팹이 생성될 부모 오브젝트
    public Button readyUp; // 준비 완료 버튼
    public TMP_Text lobbyNameText; // 로비 이름(방 코드)을 표시하는 텍스트

    // 플레이어와 관련된 UI 아이템을 저장할 사전
    private static readonly Dictionary<RoomPlayer, LobbyItemUI> ListItems = new Dictionary<RoomPlayer, LobbyItemUI>();

    // UI가 구독되었는지 여부를 확인하는 변수
    private static bool IsSubscribed;

    // 이 메소드는 UI가 활성화될 때 호출됩니다.
    private void Awake()
    {
        // 게임 매니저의 로비 세부 사항이 업데이트되면 호출될 메소드를 설정합니다.
        GameManager.OnLobbyDetailsUpdated += UpdateDetails;

        // 플레이어의 상태가 변경될 때 호출될 메소드를 설정합니다.
        RoomPlayer.PlayerChanged += (player) =>
        {
            var isLeader = RoomPlayer.Local.IsLeader; // 로컬 플레이어가 방장인지 확인합니다.
        };
    }

    // 게임 매니저의 세부 사항이 업데이트되면 호출되는 메소드
    void UpdateDetails(GameManager manager)
    {
        // 로비 이름(방 코드)을 업데이트합니다.
        lobbyNameText.text = "Room Code: " + manager.LobbyName;
    }

    // 이 메소드는 UI를 설정할 때 호출됩니다.
    public void Setup()
    {
        if (IsSubscribed) return; // 이미 구독된 경우, 중복 구독을 방지합니다.

        // 플레이어가 방에 참여하거나 나갈 때 호출될 메소드를 설정합니다.
        RoomPlayer.PlayerJoined += AddPlayer;
        RoomPlayer.PlayerLeft += RemovePlayer;
        
        // 플레이어의 상태가 변경될 때 호출될 메소드를 설정합니다.
        RoomPlayer.PlayerChanged += EnsureAllPlayersReady;
        

        // 준비 완료 버튼에 클릭 리스너를 추가합니다.
        readyUp.onClick.AddListener(ReadyUpListener);

        IsSubscribed = true; // 구독 상태를 설정합니다.
    }

    // 게임 오브젝트가 파괴될 때 호출되는 메소드
    private void OnDestroy()
    {
        if (!IsSubscribed) return; // 구독되지 않은 경우, 아무 작업도 하지 않습니다.

        // 플레이어가 방에 참여하거나 나갈 때 호출될 메소드를 제거합니다.
        RoomPlayer.PlayerJoined -= AddPlayer;
        RoomPlayer.PlayerLeft -= RemovePlayer;

        // 준비 완료 버튼의 클릭 리스너를 제거합니다.
        readyUp.onClick.RemoveListener(ReadyUpListener);

        IsSubscribed = false; // 구독 상태를 해제합니다.
    }

    // 플레이어가 방에 참여할 때 호출되는 메소드
    private void AddPlayer(RoomPlayer player)
    {
        if (ListItems.ContainsKey(player))
        {
            var toRemove = ListItems[player];
            Destroy(toRemove.gameObject); // 기존 UI 아이템을 제거합니다.

            ListItems.Remove(player); // 사전에서 플레이어를 제거합니다.
        }

        var obj = Instantiate(textPrefab, parent).GetComponent<LobbyItemUI>(); // 새로운 UI 아이템을 생성합니다.
        obj.SetPlayer(player); // UI 아이템에 플레이어 정보를 설정합니다.

        ListItems.Add(player, obj); // 사전에 새로운 UI 아이템을 추가합니다.
        
        UpdateDetails(GameManager.Instance); // UI를 업데이트합니다.
    }

    // 플레이어가 방을 나갈 때 호출되는 메소드
    private void RemovePlayer(RoomPlayer player)
    {
        if (!ListItems.ContainsKey(player))
            return; // 플레이어가 목록에 없으면 아무 작업도 하지 않습니다.

        var obj = ListItems[player];
        if (obj != null)
        {
            Destroy(obj.gameObject); // UI 아이템을 제거합니다.
            ListItems.Remove(player); // 사전에서 플레이어를 제거합니다.
        }
    }

    // 이 메소드는 UI가 파괴될 때 호출됩니다.
    public void OnDestruction()
    {
        // 현재는 아무 작업도 하지 않습니다.
    }

    // 준비 완료 버튼이 클릭될 때 호출되는 메소드
    private void ReadyUpListener()
    {
        var local = RoomPlayer.Local; // 로컬 플레이어를 가져옵니다.
        if (local && local.Object && local.Object.IsValid)
        {
            local.RPC_ChangeReadyState(!local.IsReady); // 로컬 플레이어의 준비 상태를 변경합니다.
        }
    }

    // 모든 플레이어가 준비 완료 상태인지 확인하는 메소드
    private void EnsureAllPlayersReady(RoomPlayer lobbyPlayer)
    {
        if (!RoomPlayer.Local.IsLeader) 
            return; // 로컬 플레이어가 방장이 아닌 경우, 아무 작업도 하지 않습니다.

        if (IsAllReady()) // 모든 플레이어가 준비 완료 상태인지 확인합니다.
        {
            ResourceManager.Instance.Assign(); //직업 할당
            // mainCanvasEventSystem.SetActive(false); //이벤트 리스너 중복 제거
            Destroy(mainCanvasEventSystem);

            Debug.Log("뭘쓰죠이벤트리스너 ???");
            // 선택된 트랙의 씬을 로드합니다.
            // int scene = ResourceManager.Instance.tracks[GameManager.Instance.TrackId].buildIndex;
            int scene = 1; // 임시
            LevelManager.LoadTrack(scene);
        }
    }

    // 모든 플레이어가 준비 완료 상태인지 확인하는 메소드
    private static bool IsAllReady() => RoomPlayer.Players.Count > 0 && RoomPlayer.Players.All(player => player.IsReady);
}