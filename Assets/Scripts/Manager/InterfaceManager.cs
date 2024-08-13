using FusionExamples.Utility;
using UnityEngine;
using TMPro;

public class InterfaceManager : MonoBehaviour
{
	// [SerializeField] private ProfileSetupUI profileSetup;

	public UIScreen mainMenu;
	public UIScreen pauseMenu;
	public UIScreen lobbyMenu;

    public Transform worldCanvasTransform;
	public Transform nicknameHolder;
	public TMP_Text tasksCountDisplay;

	public static InterfaceManager Instance => Singleton<InterfaceManager>.Instance;

	private void Start()
	{
		// profileSetup.AssertProfileSetup();
	}

	public void OpenPauseMenu()
	{
		// open pause menu only if the kart can drive and the menu isn't open already
		if (UIScreen.activeScreen != pauseMenu)
		{
			UIScreen.Focus(pauseMenu);
		}
	}

	public void Quit()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#endif
		Application.Quit();
	}

	// Audio Hooks
	public void SetVolumeMaster(float value) => AudioManager.SetVolumeMaster(value);
	public void SetVolumeSFX(float value) => AudioManager.SetVolumeSFX(value);
	public void SetVolumeUI(float value) => AudioManager.SetVolumeUI(value);
	public void SetVolumeMusic(float value) => AudioManager.SetVolumeMusic(value);
}