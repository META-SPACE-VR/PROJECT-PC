using System.Collections;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class KeyBoard : MonoBehaviour
{
    public List<Button> buttons; // 버튼 목록
    public Button enterButton; // 엔터 버튼
    public Button backspaceButton; // 백스페이스 버튼

    [SerializeField]
    GameObject keyBoard;

    [SerializeField]
    DisplayUIManager displayUIMgr;

    [SerializeField]
    AudioSource typingSound;

    [SerializeField]
    AudioSource enterKeytypingSound;


    void Start()
    {
        foreach (Button button in buttons)
        {
            if (button != null)
            {
                string character = button.name; // Use button's name instead of text
                button.onClick.AddListener(() => OnButtonClick(character));
                Debug.Log("버튼 할당됨: " + button.name);
            }
            else
            {
                Debug.LogError("버튼 목록에 null 값이 있습니다.");
            }
        }

        enterButton.onClick.AddListener(CheckAnswer);
        backspaceButton.onClick.AddListener(RemoveCharacter);
    }

    public void AddCharacter(string character)
    {   
        typingSound.Play();
        displayUIMgr.AddCharacter(character);
    }

    public void CheckAnswer()
    {   
        enterKeytypingSound.Play();
        displayUIMgr.CheckAnswer();
    }

    public void RemoveCharacter()
    {
        typingSound.Play();
        displayUIMgr.RemoveCharacter();
    }

    public void OnButtonClick(string character)
    {
        Debug.Log("버튼 클릭됨, 문자: " + character);
        AddCharacter(character);
    }
}
