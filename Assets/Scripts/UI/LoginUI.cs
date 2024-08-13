using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoginUI : PageUI
{
    [SerializeField]
    DisplayUIManager displayUiMgr; // Control UI Manager
    [SerializeField]
    GameObject wrongPanel; // 비밀번호가 틀릴 때 표시할 패널

    IEnumerator wrongPanelCoroutine; // 실행중인 오답 패널 코루틴

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
}
