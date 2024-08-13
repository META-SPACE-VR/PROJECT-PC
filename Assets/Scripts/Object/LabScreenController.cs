using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LabScreenController : MonoBehaviour
{
    public string answer;
    public TextMeshProUGUI password;
    public GameObject keyboard;
    public Renderer screen;
    public Material labPasswordSuccessMaterial;
    public bool isSucceeded = false;

    private string code = "";

    public void InputKey(string keyName)
    {
        if (code.Length < answer.Length)
        {
            code += keyName;
            password.text = code;
        }
    }

    public void BackspaceKey()
    {
        if (code.Length > 0)
        {
            code = code.Substring(0, code.Length - 1);
            password.text = code;
        }
    }

    public void EnterKey()
    {
        if (code == answer)
        {
            screen.material = labPasswordSuccessMaterial;
            isSucceeded = true;
            password.gameObject.SetActive(false);
            keyboard.SetActive(false);
        }
        else
        {
            StartCoroutine(ShowWrongPasswordMessage());
        }
    }

    private IEnumerator ShowWrongPasswordMessage()
    {
        password.text = "패스워드가 틀렸습니다";
        password.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        password.text = "";
        password.color = Color.black;
        code = "";
    }
}
