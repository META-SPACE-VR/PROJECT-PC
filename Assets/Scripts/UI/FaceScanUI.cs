using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceScanUI : PageUI
{
    [SerializeField]
    GameObject scanVideo;

    [SerializeField]
    GameObject nonVerifiedDescription;

    [SerializeField]
    GameObject verifiedDescription;

    [SerializeField]
    GameObject nonVerifiedButton;

    [SerializeField]
    GameObject verifiedButton;

    bool isVerified = false;

    protected void OnEnable() {
        base.OnEnable();
        Reset();
    }

    // 상태 초기화
    void Reset() {
        isVerified = false;
        nonVerifiedDescription.SetActive(true);
        verifiedDescription.SetActive(false);
        nonVerifiedButton.SetActive(true);
        verifiedButton.SetActive(false);
    }

    // 얼굴 스캔
    public void FaceScan() {
        scanVideo.SetActive(true);
    }

    // 인증 성공
    public void VerifySuccess() {
        isVerified = true;
        nonVerifiedDescription.SetActive(false);
        verifiedDescription.SetActive(true);
        nonVerifiedButton.SetActive(false);
        verifiedButton.SetActive(true);
    }
}
