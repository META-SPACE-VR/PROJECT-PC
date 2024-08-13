using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadePanel : MonoBehaviour
{   
    // 사용하는 판넬 이미지
    [SerializeField]
    Image panel;

    // 페이드 소요 시간
    [SerializeField]
    float fadeTime;
    
    // 페이드 진행 시간
    float currentTime = 0f;

    // 페이드 인 과정
    public IEnumerator FadeIn() {
        Debug.Log("Fade In");

        // 패널 색
        Color color = panel.color;

        // 패널이 완벽히 사라질 때까지 alpha값을 점진적으로 증가
        while(color.a > 0f) {
            currentTime += Time.deltaTime;
            color.a = Mathf.Lerp(1, 0, currentTime / fadeTime);
            panel.color = color;
            yield return null;
        }

        // 패널 비활성화
        panel.gameObject.SetActive(false);

        // currentTime 초기화
        currentTime = 0f;

        yield return null;
    }

    // 페이드 아웃 과정
    public IEnumerator FadeOut() {
        Debug.Log("Fade Out");

        // 패널 활성화
        panel.gameObject.SetActive(true);

        // 패널 색
        Color color = panel.color;

        // 패널이 완벽히 보일때까지 alpha값을 점진적으로 증가
        while(color.a < 1f) {
            currentTime += Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, currentTime / fadeTime);
            panel.color = color;
            yield return null;
        }

        // currentTime 초기화
        currentTime = 0f;

        yield return null;
    }
}
