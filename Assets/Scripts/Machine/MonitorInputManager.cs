using UnityEngine;

public class MonitorInputManager : MonoBehaviour
{
    public Camera monitorCamera; // 모니터 화면을 렌더링하는 카메라
    public RenderTexture monitorTexture; // 모니터 화면에 사용할 렌더 텍스처
    public Material highlightMaterial; // 강조할 셰이더가 적용된 재질
    private bool isDrawing = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            if (IsPointOnMonitor(mousePosition))
            {
                Debug.Log("Mouse Down on Monitor at " + mousePosition);
                isDrawing = true;
                SetHighlightPosition(mousePosition);
            }
        }

        if (Input.GetMouseButtonUp(0) && isDrawing)
        {
            Debug.Log("Mouse Up");
            isDrawing = false;
        }
    }

    bool IsPointOnMonitor(Vector3 point)
    {
        // 모니터 화면의 범위 안에 있는지 확인합니다.
        // 이 부분은 모니터 화면의 좌표 범위로 변경해야 할 수도 있습니다.
        return point.x >= 0 && point.x <= Screen.width && point.y >= 0 && point.y <= Screen.height;
    }

    void SetHighlightPosition(Vector3 screenPoint)
    {
        // 모니터 카메라의 뷰포트 좌표로 변환합니다.
        Vector3 viewportPoint = monitorCamera.ScreenToViewportPoint(screenPoint);
        // 뷰포트 좌표를 월드 좌표로 변환합니다.
        Vector3 worldPoint = monitorCamera.ViewportToWorldPoint(new Vector3(viewportPoint.x, viewportPoint.y, monitorCamera.nearClipPlane));

        // 디버그 로그 추가
        Debug.Log("Setting Highlight Position to " + worldPoint);

        // 셰이더에 강조할 위치와 반경 설정
        highlightMaterial.SetVector("_HighlightPosition", new Vector4(worldPoint.x, worldPoint.y, worldPoint.z, 1));
        highlightMaterial.SetFloat("_HighlightRadius", 0.1f); // 강조할 반경 설정

        // 셰이더 매개변수 확인
        Debug.Log("Highlight Position: " + highlightMaterial.GetVector("_HighlightPosition"));
        Debug.Log("Highlight Radius: " + highlightMaterial.GetFloat("_HighlightRadius"));
    }
}
