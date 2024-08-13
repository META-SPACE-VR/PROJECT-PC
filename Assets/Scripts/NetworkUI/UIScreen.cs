using UnityEngine;

public class UIScreen : MonoBehaviour
{
    public bool isModal = false; 
    
    //[SerializeField]는 Unity에서 직렬화를 허용하고 인스펙터 창에서 해당 변수를 접근할 수 있게 해주는 어트리뷰트
    [SerializeField] private UIScreen previousScreen = null; 

    public static UIScreen activeScreen;

    public static void Focus(UIScreen screen)
    {
        // 주어진 screen이 이미 활성화된 상태면 return 
        if (screen==activeScreen) return;

        // 현재 활성화된 screen이 존재하면 해당 screen 비활성화 
        if (activeScreen) activeScreen.Defocus();

        // 현재 활성화된 screen을 이전 화면으로 설정하고, 새로운 screen을 활성화된 화면으로 설정
        screen.previousScreen = activeScreen;
        activeScreen = screen;

        // 새로운 화면을 활성화
        screen.Focus();
    }

    //현재 activeScreen이 null이 아니면 BackTo(null)을 호출하여 초기화 
    public static void BackToInitial() 
    {
        activeScreen?.BackTo(null);
    }

    //주어진 screen에 Focus 
    public void FocusScreen(UIScreen screen)
    {
        Focus(screen);
    }

    //gameObject가 존재하면 활성화 
    private void Focus()
    {
        if(gameObject) gameObject.SetActive(true);
    }
    
    //gameObject가 존재하면 비활성화 
    private void Defocus()
    {
        if(gameObject) gameObject.SetActive(false);
    }
    
    //이전 화면이 존재할 경우 돌아감 
    public void Back()
    {
        if (previousScreen)
        {
            Defocus();
            activeScreen = previousScreen;
            activeScreen.Focus();
            previousScreen = null;
        }
    }

    //주어진 screen이 활성화되기 전까지 이전 화면으로 계속해서 되돌아가는 역할 
    //현재 활성화된 스크린이 있고, 이전에 활성화했던 스크린이 존재하고, activeScreen이 주어진 screen과 같지 않은 동안 반복 
    public void BackTo(UIScreen screen)
    {
        while (activeScreen!=null && activeScreen.previousScreen!=null && activeScreen!=screen)
        activeScreen.Back();
    }
}
