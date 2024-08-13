using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class HolePair {
    public int displayIndex;
    public GameObject cableHole1;
    public GameObject cableHole2;

    public HolePair(int _displayIndex, GameObject _cableHole1, GameObject _cableHole2) {
        displayIndex = _displayIndex;
        cableHole1 = _cableHole1;
        cableHole2 = _cableHole2;
    }
}

public class PanelManager : MonoBehaviour
{
    // 디스플레이
    [SerializeField]
    List<Display> displays;

    // 각 디스플레이에 대응되는 올바른 케이블 연결
    [SerializeField]
    List<HolePair> answer;

    List<Dictionary<GameObject, GameObject>> answerDicts;

    // 사용하는 케이블들 (플레이어가 직접 손대는)
    [SerializeField]
    List<Cable> cables;

    // 동작까지 남은 연결 수
    public int[] remain;

    void Awake() {
        remain = new int[displays.Count];
        answerDicts = new List<Dictionary<GameObject, GameObject>>();
        for(int i = 0; i < displays.Count; i++) {
            answerDicts.Add(new Dictionary<GameObject, GameObject>());
        }

        // answerDicts와 remain 초기화
        foreach(HolePair holePair in answer) {
            int displayIndex = holePair.displayIndex;
            answerDicts[displayIndex].Add(holePair.cableHole1, holePair.cableHole2);
            answerDicts[displayIndex].Add(holePair.cableHole2, holePair.cableHole1);
            remain[displayIndex]++;
        }

        for(int i = 0; i < displays.Count; i++) {
            if(remain[i] > 0) displays[i].TurnOff();
            else displays[i].TurnOn();
        }
    }

    // 올바른 연결 쌍인지 확인
    int CheckCorrectPair(GameObject cableHole1, GameObject cableHole2) {
        for(int i = 0; i < displays.Count; i++) {
            if(answerDicts[i].TryGetValue(cableHole1, out GameObject target) && target == cableHole2) {
                return i;
            }
        }

        return -1;
    }

    // 두 구멍이 연결됨
    public void ConnectPair(GameObject cableHole1, GameObject cableHole2) {
        int displayIndex = CheckCorrectPair(cableHole1, cableHole2);
        if(cableHole1 && cableHole2 && displayIndex != -1) {
            remain[displayIndex]--;
            if(remain[displayIndex] == 0) {
                displays[displayIndex].TurnOn();
            }
        }
    }

    // 두 구멍이 연결 해제 됨
    public void DisconnectPair(GameObject cableHole1, GameObject cableHole2) {
        int displayIndex = CheckCorrectPair(cableHole1, cableHole2);
        if(cableHole1 && cableHole2 && displayIndex != -1) {
            if(remain[displayIndex] == 0) {
                displays[displayIndex].TurnOff();
            }
            remain[displayIndex]++;
        }
    }
}
