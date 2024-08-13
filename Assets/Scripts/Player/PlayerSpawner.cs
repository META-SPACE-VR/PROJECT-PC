using Fusion;
using UnityEngine;

/// <summary>
/// SimulationBehaviour that spawns and despawns players as they join and leave.
/// </summary>
public class PlayerSpawner : SimulationBehaviour, IPlayerLeft
{	
    [Tooltip("Prefab of that character that will be spawned.")]
    public NetworkObject playerObject;

    // public void SceneLoadDone(여기에 뭔가 들어가야 하는데 , , , , )
    // {
    //     Debug.Log("이거 되니");
    //     if (Runner.IsServer)
    //     {
    //         Runner.Spawn(playerObject, Vector3.zero, inputAuthority: player);
    //     }
    // }

    public void PlayerLeft(PlayerRef player)
    {
        Debug.Log("플레이어 나감");
        // Only the server can despawn.
        // if (Runner.IsServer)
        // {
        //     PlayerObject leftPlayer = PlayerRegistry.GetPlayer(player);
        //     if (leftPlayer != null)
        //     {
        //         Runner.Despawn(leftPlayer.Object);
        //     }
        // }
    }
    
}