using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisconnectUI : MonoBehaviour
{
	public Transform parent;
	public TMP_Text disconnectStatus;
	public TMP_Text disconnectMessage;

	public void ShowMessage( string status, string message)
	{
		if (status == null || message == null)
			return;

		disconnectStatus.text = status;
		disconnectMessage.text = message;

		Debug.Log($"Showing message({status},{message})");
		parent.gameObject.SetActive(true);
	}
}