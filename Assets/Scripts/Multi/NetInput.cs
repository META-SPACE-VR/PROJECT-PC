using Fusion;
using UnityEngine;

public enum InputButton
{
    Jump,
    Run,  // Run 버튼 추가
    Interact,
    Trigger,
    //--------------- relative robot arm ---------------
    RobotUp,
    RobotDown,
    RobotLeft,
    RobotRight,
    RobotAttach,
    //--------------- relative robot arm ---------------
    Qtrigger
}

public struct NetInput : INetworkInput
{
    public NetworkButtons Buttons;
    public Vector2 Direction;
    public Vector2 LookDelta;
}
