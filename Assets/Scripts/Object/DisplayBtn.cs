using UnityEngine;
using UnityEngine.UI;

public class DisplayBtn : MonoBehaviour
{
    public GameObject LoginUi;
    public Button logoBtn;

    // Start is called before the first frame update
    void Start()
    {
        LoginUi.SetActive(false); // 처음에는 로그인 UI 없이
    }
}
