using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizUI : PageUI
{
    [SerializeField]
    DisplayUIManager displayUiMgr; // Display UI Manager
    [SerializeField]
    GameObject wrongPanel; // 비밀번호가 틀릴 때 표시할 패널
    [SerializeField]
    OpenSecondFloorEvent successEvent; // 성공시 발생하는 이벤트

    IEnumerator wrongPanelCoroutine; // 실행중인 오답 패널 코루틴

    public GameObject mainUI;
    public GameObject quiz1UI;
    public GameObject quiz2UI;

    public void SuccessAction()
    {
        successEvent.PlayEvent();
    }

    public void FailedAction()
    {
        // 이미 작동중인 코루틴이 있으면 중지
        if(wrongPanelCoroutine != null) {
            wrongPanel.SetActive(false);
            StopCoroutine(wrongPanelCoroutine);
        }
        
        // 오답 패널을 띄운다
        wrongPanelCoroutine = ShowWrongPanel();
        StartCoroutine(wrongPanelCoroutine);
    }

    // 오답 패널을 보여주는 코루틴
    IEnumerator ShowWrongPanel() {
        wrongPanel.SetActive(true);
        wrongSound.Play();
        yield return new WaitForSeconds(1f);
        wrongPanel.SetActive(false);
        yield return null;
    }

    // 메인 탭으로 전환
    public void ShowMain()
    {
        mainUI.SetActive(true);
        quiz1UI.SetActive(false);
        quiz2UI.SetActive(false);
    }

    // 퀴즈 1 탭으로 전환
    public void ShowQuiz1()
    {
        mainUI.SetActive(false);
        quiz1UI.SetActive(true);
        quiz2UI.SetActive(false);
    }

    // 퀴즈 2 탭으로 전환
    public void ShowQuiz2()
    {
        mainUI.SetActive(false);
        quiz1UI.SetActive(false);
        quiz2UI.SetActive(true);
    }
}
