using Fusion;
using UnityEngine;

public class GlassDoor : NetworkBehaviour
{
    [Networked] private bool isClosed { get; set; } = true;

    // 문 상태에 따라 문을 열거나 닫는 로직
    public void ToggleDoor()
    {
        // 상태를 네트워크 상에서 동기화
        isClosed = !isClosed;
        UpdateDoorVisual();
    }

    // 문 상태에 따른 시각적 업데이트
    private void UpdateDoorVisual()
    {
        if (isClosed)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);  // 문이 닫힌 상태
        }
        else
        {
            transform.eulerAngles = new Vector3(0, -90, 0);  // 문이 열린 상태
        }
    }

    // 네트워크에서 상태를 지속적으로 업데이트
    public override void FixedUpdateNetwork()
    {
        UpdateDoorVisual();
    }
}
