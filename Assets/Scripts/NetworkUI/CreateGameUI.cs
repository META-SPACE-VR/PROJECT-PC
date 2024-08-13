using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using TMPro;

public class CreateGameUI : MonoBehaviour
{
    public TMP_InputField lobbyName; 
    public Button confirmButton; 
    
	private void Start()
	{

		lobbyName.onValueChanged.AddListener(x =>
		{
			ServerInfo.LobbyName = x;
			confirmButton.interactable = !string.IsNullOrEmpty(x);
		});
		lobbyName.text = ServerInfo.LobbyName = CreateRoomCode();
		// lobbyName.text = ServerInfo.LobbyName = Random.Range(0, 1000).ToString();

	}

	// UI Hooks

    private bool _lobbyIsValid;

	public void ValidateLobby()
	{
		_lobbyIsValid = string.IsNullOrEmpty(ServerInfo.LobbyName) == false;
	}

	public void TryFocusScreen(UIScreen screen)
	{
		if (_lobbyIsValid)
		{
			UIScreen.Focus(screen);
		}
	}

	public void TryCreateLobby(GameLauncher launcher)
	{
		if (_lobbyIsValid)
		{
			launcher.JoinOrCreateLobby();
			_lobbyIsValid = false;
		}
	}

	public static string CreateRoomCode(int length = 4)
	{
		char[] chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789".ToCharArray();

		string str = "";
		for (int i = 0; i < length; i++)
		{
			str += chars[Random.Range(0, chars.Length)];
		}
		return str;
	}
}