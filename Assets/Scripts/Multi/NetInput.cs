using Fusion;
using UnityEngine;

public enum InputButton
{
    Jump,
    Run,  // Run 버튼 추가
    Interact,
    Trigger,
    Transform
}

public struct NetInput : INetworkInput
{
    public NetworkButtons Buttons;
    public Vector2 Direction;
    public Vector2 LookDelta;
}
