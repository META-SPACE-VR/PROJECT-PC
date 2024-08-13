using UnityEngine;
using UnityEngine.SceneManagement;

public static class ServerInfo {

    public const int UserCapacity = 4; 

    public static string LobbyName;

    public static int GameMode {
        get => PlayerPrefs.GetInt("S_GameMode", 0);
        set => PlayerPrefs.SetInt("S_GameMode", value);
    }

    public static int MaxUsers {
        get => PlayerPrefs.GetInt("S_MaxUsers", 4);
        set => PlayerPrefs.SetInt("S_MaxUsers", Mathf.Clamp(value, 1, UserCapacity));
    }
}