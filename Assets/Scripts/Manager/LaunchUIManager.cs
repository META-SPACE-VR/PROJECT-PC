using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LaunchUIManager : MonoBehaviour
{

    [Serializable]
    public class UIData {
        public LaunchUIType uiType;
        public GameObject ui;
        public UIData(LaunchUIType _uiType, GameObject _ui) {
            uiType = _uiType;
            ui = _ui;
        }
    }

    LaunchUIType currentType = LaunchUIType.NickName; // 현재 UI 타입
    
    [SerializeField]
    List<UIData> uiDatas; // UI 목록 (List)

    readonly Dictionary<LaunchUIType, GameObject> uiDict = new(); // UI 목록 (Dictionary)

    // Start is called before the first frame update
    void Start()
    {   
        foreach (UIData uiData in uiDatas) {
            uiDict.Add(uiData.uiType, uiData.ui);
        }

        ChangeUI(LaunchUIType.NickName);
    }
    
    // UI 변경
    void ChangeUI(LaunchUIType nextType) {
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

    public void NavigateNickNameUI() { ChangeUI(LaunchUIType.NickName); }
    public void NavigateRoomCodeUI() { ChangeUI(LaunchUIType.RoomCode); }

    // 현재 페이지의 활성화된 InputField에 문자 추가
    public void AddCharacter(string character) {
        uiDict[currentType].GetComponent<LaunchUI>().AddCharacter(character);
    }

    // 현재 페이지의 활성화된 InputField의 맨 뒤 문자 제거
    public void RemoveCharacter()
    {
        uiDict[currentType].GetComponent<LaunchUI>().RemoveCharacter();
    }
}
