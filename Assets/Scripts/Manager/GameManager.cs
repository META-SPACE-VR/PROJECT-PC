using System;
using Fusion;
using UnityEngine;

// 이 클래스는 싱글톤 패턴을 사용해서 게임 전체에서 하나의 인스턴스만 존재하도록 보장하고 다양한 게임 설정을 관리 

public class GameManager : NetworkBehaviour
{
    // 게임 로비의 세부 정보가 업데이트되었을 때 호출되는 이벤트
    public static event Action<GameManager> OnLobbyDetailsUpdated;

    // 싱글톤 패턴으로 인스턴스를 관리
    public static GameManager Instance { get; private set; }

    // 리소스 매니저를 저장
    public static ResourceManager rm { get; private set; }
    // 인터페이스 매니저를 저장
    public static InterfaceManager im { get; private set; }
    // 음성 매니저를 저장
    // public static VoiceManager vm { get; private set; }

    public static Map CurrentMap { get; private set; }
    // public static bool IsPlaying => CurrentMap != null;

    // 게임 설정을 저장하고 변경될 때 렌더링 함수 호출
    // [Networked, OnChangedRender(nameof(GameSettingsChanged))]
    // public GameSettings Settings { get; set; } = GameSettings.Default;

    // 완료된 작업 수를 저장하고 변경될 때 렌더링 함수 호출
    // [Networked, OnChangedRender(nameof(TasksCompletedChanged))]
    // public byte TasksCompleted { get; set; }

    // 작업 표시 목록을 저장
    // public List<TaskStation> taskDisplayList;
    // 작업 표시 수량을 저장
    // public readonly Dictionary<TaskBase, byte> taskDisplayAmounts = new Dictionary<TaskBase, byte>();

    // 네트워크 변수들
    [Networked] public NetworkString<_32> LobbyName { get; set; } // 네트워크에서 로비 이름을 저장
    [Networked] public int GameTypeId { get; set; } // 네트워크에서 게임 타입 ID를 저장
    [Networked] public int MaxUsers { get; set; } // 네트워크에서 최대 사용자 수를 저장

    // 로비 세부 정보가 변경되었을 때 호출되는 콜백 메소드
    private static void OnLobbyDetailsChangedCallback(GameManager changed)
    {
        OnLobbyDetailsUpdated?.Invoke(changed); // 이벤트를 호출하여 로비 세부 정보 업데이트를 알림
    }

    // 변경 감지기를 사용하는 변수
    private ChangeDetector _changeDetector;

    // 게임 매니저가 깨어날 때 호출되는 함수
    private void Awake()
    {
        // 싱글톤 패턴: 이미 인스턴스가 있으면 새로운 객체를 생성하지 않음
        if (Instance)
        {
            Destroy(gameObject); // 기존 객체를 파괴
            return;
        }
        Instance = this; // 현재 객체를 인스턴스로 설정
        DontDestroyOnLoad(gameObject); // 씬 전환 시에도 파괴되지 않도록 설정
    }

    // 네트워크에서 객체가 스폰되었을 때 호출되는 메소드
    public override void Spawned()
    {
        base.Spawned();

        // Voice Manager initialize
        // vm.Init(
        //     Runner.GetComponent<Photon.Voice.Unity.VoiceConnection>(),
        //     Runner.GetComponentInChildren<Photon.Voice.Unity.Recorder>()
        // );

        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState); // 변경 감지기 초기화

        if (Object.HasStateAuthority) // 현재 객체가 상태 권한이 있는 경우
        {
            LobbyName = ServerInfo.LobbyName;
            GameTypeId = ServerInfo.GameMode;
            MaxUsers = ServerInfo.MaxUsers;
        }
    }

    // 프레임 렌더링 중에 변경 사항을 감지하고 처리하는 메소드
    public override void Render()
    {
        foreach (var change in _changeDetector.DetectChanges(this)) // 변경 사항 감지
        {
            switch (change)
            {
                case nameof(LobbyName):
                case nameof(GameTypeId):
                case nameof(MaxUsers):
                    OnLobbyDetailsChangedCallback(this); // 로비 세부 정보 변경 콜백 호출
                    break;
            }
        }
    }


	public static void SetMap(Map map)
	{
		CurrentMap = map;
	}
}