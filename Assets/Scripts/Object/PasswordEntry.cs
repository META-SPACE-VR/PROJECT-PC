using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PasswordEntry : MonoBehaviour
{
    public TMP_Text textInput; // 텍스트 입력 표시를 위한 TMP_Text
    public string correctPassword; // 올바른 비밀번호 설정
    public GameObject newScreen; // 올바른 비밀번호일 때 표시할 UI 요소
    public GameObject wrongPanel; // 비밀번호가 틀릴 때 표시할 패널
    public List<Button> buttons; // 버튼 목록
    public Button enterButton; // 엔터 버튼
    public Button backspaceButton; // 백스페이스 버튼
    public TriggerArea triggerArea; // TriggerArea 스크립트 참조
    public GameObject examinationPanel; // 새로운 검사 패널 참조
    public Animator doorAnimator; // 도어 애니메이터 참조

    private string currentInput = "";

    void Start()
    {
        if (textInput == null) Debug.LogError("텍스트 입력 필드가 할당되지 않았습니다.");
        if (newScreen == null) Debug.LogError("새로운 화면이 할당되지 않았습니다.");
        if (wrongPanel == null) Debug.LogError("잘못된 비밀번호 패널이 할당되지 않았습니다.");
        if (buttons == null || buttons.Count == 0) Debug.LogError("버튼이 할당되지 않았습니다.");
        if (triggerArea == null) Debug.LogError("Trigger Area가 할당되지 않았습니다.");
        if (examinationPanel == null) Debug.LogError("검사 패널이 할당되지 않았습니다.");
        if (enterButton == null) Debug.LogError("엔터 버튼이 할당되지 않았습니다.");
        if (backspaceButton == null) Debug.LogError("백스페이스 버튼이 할당되지 않았습니다.");

        newScreen.SetActive(false); // 처음에는 새로운 화면을 숨김
        examinationPanel.SetActive(false); // 처음에는 검사 패널을 숨김
        wrongPanel.SetActive(false); // 처음에는 잘못된 비밀번호 패널을 숨김

        UpdateDisplay();

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

        enterButton.onClick.AddListener(CheckPassword);
        backspaceButton.onClick.AddListener(RemoveCharacter);
    }

    // 현재 입력에 문자를 추가하는 메서드
    public void AddCharacter(string character)
    {
        Debug.Log("문자 추가됨: " + character);
        if (currentInput.Length < correctPassword.Length)
        {
            currentInput += character;
            UpdateDisplay();
            wrongPanel.SetActive(false); // 새로운 문자가 추가될 때마다 잘못된 비밀번호 패널을 숨김
        }
    }

    // 현재 입력에서 마지막 문자를 제거하는 메서드
    public void RemoveCharacter()
    {
        Debug.Log("마지막 문자 제거됨");
        if (currentInput.Length > 0)
        {
            currentInput = currentInput.Substring(0, currentInput.Length - 1);
            UpdateDisplay();
            wrongPanel.SetActive(false); // 문자가 제거될 때마다 잘못된 비밀번호 패널을 숨김
        }
    }

    // 입력한 비밀번호가 올바른지 확인하는 메서드
    public void CheckPassword()
    {
        Debug.Log("비밀번호 확인됨, 현재 입력: " + currentInput);
        if (currentInput == correctPassword)
        {
            Debug.Log("비밀번호 일치!");
            SwitchToNewScreen();
            if (doorAnimator != null)
            {
                doorAnimator.SetTrigger("OpenDoor"); // 정답을 맞추면 애니메이션 트리거 호출
                doorAnimator.SetTrigger("character_nearby");
            }
        }
        else
        {
            Debug.Log("비밀번호 불일치!");
            wrongPanel.SetActive(true); // 잘못된 비밀번호 패널을 표시
        }
    }

    // 새로운 화면으로 전환하는 메서드
    private void SwitchToNewScreen()
    {
        newScreen.SetActive(true);
        wrongPanel.SetActive(false); // 새로운 화면으로 전환할 때 잘못된 비밀번호 패널을 숨김

        // 로그인 화면 비활성화
        textInput.transform.parent.gameObject.SetActive(false);

        // 검사 패널 활성화
        examinationPanel.SetActive(true);

        // 트리거 상태 벗어나기
        triggerArea.ExitInteraction();
    }

    // 디스플레이 텍스트를 업데이트하는 메서드
    private void UpdateDisplay()
    {
        textInput.text = currentInput;
    }

    // 버튼 클릭을 처리하는 메서드
    public void OnButtonClick(string character)
    {
        Debug.Log("버튼 클릭됨, 문자: " + character);
        AddCharacter(character);
    }
}