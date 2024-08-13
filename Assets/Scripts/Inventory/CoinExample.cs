using Fusion;
using UnityEngine;

// 이 클래스는 네트워크 게임에서 코인을 관리합니다. 코인은 충돌할 때 플레이어에 추가되고 비활성화됩니다.
public class CoinExample : NetworkBehaviour {

    // 네트워크에서 이 코인이 활성화 상태인지 아닌지를 저장합니다.
    [Networked]
    public NetworkBool IsActive { get; set; } = true;

    // 코인의 모습(모양)을 나타내는 객체입니다.
    public Transform visuals;
    
    // 상태 변화 감지를 위한 도구입니다.
    private ChangeDetector _changeDetector;

    // 코인이 생성될 때 호출되는 함수입니다.
    public override void Spawned()
    {
        // 상태 변화 감지기를 초기화합니다.
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    // 코인의 상태를 렌더링(화면에 보여줄)할 때 호출되는 함수입니다.
    public override void Render()
    {
        // 상태 변화 감지기를 사용해 변화를 탐지합니다.
        foreach (var change in _changeDetector.DetectChanges(this))
        {
            // 상태가 바뀌었을 때 처리합니다.
            switch (change)
            {
                case nameof(IsActive):
                    OnIsEnabledChangedCallback(this);
                    break;
            }
        }
    }

    // 플레이어가 이 코인과 충돌할 때 호출되는 함수입니다.
    public bool Collide(RoomPlayer player) {

        // 코인 객체가 없거나 네트워크에서 유효하지 않은 경우 충돌을 처리하지 않습니다.
        if (Object == null || !Runner.Exists(Object))
            return false;

        // 코인이 활성화 상태일 때만 충돌을 처리합니다.
        if (IsActive) {
            // 플레이어의 코인 개수를 하나 증가시킵니다.
            // player.CoinCount++;

            // 코인을 비활성화합니다.
            IsActive = false;
            
            // 플레이어가 상태 권한을 가지고 있을 때 코인을 네트워크에서 제거합니다.
            if ( player.Object.HasStateAuthority ) {
                Runner.Despawn(Object);
            }
        }

        // 충돌 처리가 성공적으로 이루어졌음을 나타냅니다.
        return true;
    }

    // 코인이 네트워크에서 제거될 때 호출되는 함수입니다.
    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);
    }

    // 코인의 활성화 상태가 변경되었을 때 호출되는 함수입니다.
    private static void OnIsEnabledChangedCallback(CoinExample changed) {
        // 코인의 모습을 활성화/비활성화 상태에 맞게 설정합니다.
        changed.visuals.gameObject.SetActive(changed.IsActive);

        // 코인이 비활성화되었을 때 사운드를 재생합니다.
        // if ( !changed.IsActive )
        //     AudioManager.PlayAndFollow("coinSFX", changed.transform, AudioManager.MixerTarget.SFX);
    }
}