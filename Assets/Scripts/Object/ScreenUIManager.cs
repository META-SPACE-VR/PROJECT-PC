using UnityEngine;
using UnityEngine.UI;

public class ScreenUIManager : MonoBehaviour
{
    public GameObject screenCanvas; // Canvas 오브젝트 참조
    public Canvas[] otherCanvases; // The other canvases you want to hide

    void Start()
    {
        screenCanvas.SetActive(false); // 처음에는 비활성화
    }

    public void ShowScreenUI()
    {
        screenCanvas.SetActive(true); // Canvas 활성화
        foreach (Canvas canvas in otherCanvases)
        {
            canvas.enabled = false;
        }
    }

    public void HideScreenUI()
    {
        screenCanvas.SetActive(false); // Canvas 비활성화
        foreach (Canvas canvas in otherCanvases)
        {
            canvas.enabled = true;
        }
    }

}
