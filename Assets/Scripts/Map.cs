using Fusion;
using UnityEngine;
public class Map : NetworkBehaviour
{
	public static Map Current { get; private set; }
	[Networked] public TickTimer StartRaceTimer { get; set; }
	// public Checkpoint[] checkpoints;
	public Transform[] spawnpoints;
	public string music = "";

    [SerializeField] private GameObject playerPrefab; 

	private void Awake()
	{
		Current = this;
		// InitCheckpoints();

		// Initialize cutscene
		// AudioManager.StopMusic();
        GameManager.SetMap(this);
		// AudioManager.PlayMusic("game");

        // GameManager.Instance.camera = Camera.main;
		// GameManager.GetCameraControl(this);
	}
	public override void Spawned()
	{
		base.Spawned();
		// if (RoomPlayer.Local.IsLeader)
		// {
		// 	StartRaceTimer = TickTimer.CreateFromSeconds(Runner, sequence.duration + 4f);
		// }
		// sequence.StartSequence();
	}
	private void OnDestroy()
	{
		GameManager.SetMap(null);
	}
	public void SpawnPlayer(NetworkRunner runner, RoomPlayer player)
	{
		var index = RoomPlayer.Players.IndexOf(player);
		var point = spawnpoints[index];
        // Vector3 pos = new(0,0,130);
        // var playerObject = runner.Spawn(prefab, Vector3.zero, Quaternion.identity, player.Object.InputAuthority);
        // var playerObject = runner.Spawn(playerPrefab, pos, Quaternion.identity, player.Object.InputAuthority);

		var playerObject = runner.Spawn(
			playerPrefab,
			point.position,
			point.rotation,
			player.Object.InputAuthority
		);
	}

	// private void InitCheckpoints()
	// {
	// 	for (int i = 0; i < checkpoints.Length; i++)
	// 	{
	// 		checkpoints[i].index = i;
	// 	}
	// }
}