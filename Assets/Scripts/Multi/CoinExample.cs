using Fusion;
using UnityEngine;

public class Coin : NetworkBehaviour, ICollidable {

    // 네트워크에서 관리되는 코인 활성화 상태 (true면 활성화됨)
    [Networked]
    public NetworkBool IsActive { get; set; } = true;

    // 코인의 시각적 표현을 담당하는 Transform
    public Transform visuals;
    
    // 상태 변화를 감지하는 ChangeDetector
    private ChangeDetector _changeDetector;

    // 오브젝트가 생성될 때 호출되는 메서드
    public override void Spawned()
    {
        // 시뮬레이션 상태의 변화를 감지하기 위한 ChangeDetector 초기화
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    // 오브젝트의 렌더링 단계에서 호출되는 메서드
    public override void Render()
    {
        // 감지된 모든 변화를 처리
        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                // IsActive 상태가 변경되었을 때 콜백 호출
                case nameof(IsActive):
                    OnIsEnabledChangedCallback(this);
                    break;
            }
        }
    }

    // 플레이어가 코인과 충돌했을 때 호출되는 메서드
    public bool Collide(Player player) {

        // 오브젝트가 null이거나 존재하지 않으면 충돌 무효
        if (Object == null || !Runner.Exists(Object))
            return false;

        // 코인이 활성화된 상태이면
        if (IsActive) {
            // 플레이어의 코인 수 증가
            player.CoinCount++;

            // 코인 비활성화
            IsActive = false;
            
            // 플레이어가 상태 권한을 가지고 있으면 오브젝트 제거
            if ( player.Object.HasStateAuthority ) {
                Runner.Despawn(Object);
            }
        }

        // 충돌이 성공적으로 처리됨을 반환
        return true;
    }

    // 오브젝트가 제거될 때 호출되는 메서드
    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        // 부모 클래스의 Despawned 메서드 호출
        base.Despawned(runner, hasState);
    }

    // IsActive 상태가 변경되었을 때 호출되는 콜백 메서드
    private static void OnIsEnabledChangedCallback(Coin changed) {
        // 코인의 시각적 활성화/비활성화 처리
        changed.visuals.gameObject.SetActive(changed.IsActive);

        // 코인이 비활성화될 때 사운드 재생
        // if ( !changed.IsActive ) AudioManager.PlayAndFollow("coinSFX", changed.transform, AudioManager.MixerTarget.SFX);
    }
}
