using Fusion;
using UnityEngine;

public enum InputButton
{
    Jump,
    Run,  // Run 버튼 추가
    Interact,
    Trigger,
    Transform,
    //--------------- relative robot arm ---------------
    RobotUp,
    RobotDown,
    RobotLeft,
    RobotRight,
    RobotAttach,
    //--------------- relative robot arm ---------------
    Qtrigger,
    Zoom
}

public struct NetInput : INetworkInput
{
    public NetworkButtons Buttons;
    public Vector2 Direction;
    public Vector2 LookDelta;
}
