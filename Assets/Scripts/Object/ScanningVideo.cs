using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class ScanningVideo : MonoBehaviour
{
    [SerializeField]
    VideoPlayer vp;

    [SerializeField]
    FaceScanUI ui;

    [SerializeField]
    AudioSource scanningSound;

    [SerializeField]
    float scanningTime;

    bool scanningDone = false;

    [SerializeField]
    AudioSource scanSuccessSound;

    [SerializeField]
    float scanSuccessTime;

    bool scanSuccessDone = false;
    
    float currentTime;

    // Start is called before the first frame update
    void Start()
    {
        // 영상이 종료될 때 VideoFinished 함수가 동작하도록 설정
        vp.loopPointReached += VideoFinished;
        currentTime = 0f;
    }

    private void Update() {
        currentTime += Time.deltaTime;
        if(!scanningDone && currentTime >= scanningTime) {
            scanningSound.Play();
            scanningDone = true;
        }

        if(!scanSuccessDone && currentTime >= scanSuccessTime) {
            scanSuccessSound.Play();
            scanSuccessDone = true;
        }
    }

    // 영상 종료 시 발생하는 함수
    void VideoFinished(VideoPlayer vp) {
        ui.VerifySuccess();
        currentTime = 0f;
        scanningDone = false;
        scanSuccessDone = false;
        // 오브젝트 비활성화
        gameObject.SetActive(false);
    }
}
