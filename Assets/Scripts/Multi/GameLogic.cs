using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : NetworkBehaviour, IPlayerJoined, IPlayerLeft
{
    [SerializeField] private NetworkPrefabRef playerPrefab;
    [Networked, Capacity(4)] private NetworkDictionary<PlayerRef, Player> Players => default;
    public Transform spawnPosition;  // 스폰 위치를 지정할 Transform

    public void PlayerJoined(PlayerRef player)
    {
        if (HasStateAuthority)
        {
            // Transform의 위치 값을 Vector3로 변환하여 전달
            Vector3 spawnPos = spawnPosition.position;
            NetworkObject playerObject = Runner.Spawn(playerPrefab, spawnPos, Quaternion.identity, player);
            Players.Add(player, playerObject.GetComponent<Player>());
        }
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (!HasStateAuthority)
            return;

        if (Players.TryGet(player, out Player playerBehaviour))
        {
            Players.Remove(player);
            Runner.Despawn(playerBehaviour.Object);
        }
    }
}
