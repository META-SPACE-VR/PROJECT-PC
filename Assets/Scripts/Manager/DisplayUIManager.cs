using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[Serializable]
public class UIData {
    public DisplayUIType uiType;
    public GameObject ui;
    public UIData(DisplayUIType _uiType, GameObject _ui) {
        uiType = _uiType;
        ui = _ui;
    }
}

public class DisplayUIManager : MonoBehaviour
{
    DisplayUIType currentType = DisplayUIType.Main; // 현재 UI 타입
    
    [SerializeField]
    List<UIData> uiDatas; // UI 목록 (List)

    readonly Dictionary<DisplayUIType, GameObject> uiDict = new(); // UI 목록 (Dictionary)

    // Start is called before the first frame update
    void Start()
    {   
        foreach (UIData uiData in uiDatas) {
            uiDict.Add(uiData.uiType, uiData.ui);
        }

        ChangeUI(DisplayUIType.Main);
    }
    
    // UI 변경
    void ChangeUI(DisplayUIType nextType) {
        // 변경하려는 타입이 현재 타입이랑 같다면 종료
        if(currentType == nextType) return;

        // 기존 UI 비활성화
        uiDict[currentType].SetActive(false);

        // currentType에 새로운 타입 반영
        currentType = nextType;

        // 새 UI 활성화
        uiDict[currentType].SetActive(true);

        Debug.Log("Change UI : " + currentType);
    }

    public void NavigateMainUI() { ChangeUI(DisplayUIType.Main); }
    public void NavigateLoginUI() { ChangeUI(DisplayUIType.Login); }
    public void NavigateQuizUI() { ChangeUI(DisplayUIType.Quiz); }
    public void NavigateFaceScanUI() { ChangeUI(DisplayUIType.FaceScan); }

    // 현재 페이지의 활성화된 InputField에 문자 추가
    public void AddCharacter(string character) {
        uiDict[currentType].GetComponent<PageUI>().AddCharacter(character);
    }

    // 현재 페이지의 활성화된 InputField의 맨 뒤 문자 제거
    public void RemoveCharacter()
    {
        uiDict[currentType].GetComponent<PageUI>().RemoveCharacter();
    }

    // 답 확인
    public void CheckAnswer()
    {
        uiDict[currentType].GetComponent<PageUI>().CheckAnswer();
    }
}
