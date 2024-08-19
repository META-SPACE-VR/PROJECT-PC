using Fusion;
using Fusion.Menu;
using Fusion.Sockets;
using MultiClimb.Menu;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : SimulationBehaviour, IBeforeUpdate, INetworkRunnerCallbacks
{
    private NetInput accumulatedInput;
    private bool resetInput;

    void IBeforeUpdate.BeforeUpdate()
    {
        if (resetInput)
        {
            resetInput = false;
            accumulatedInput = default;
        }

        Keyboard keyboard = Keyboard.current;
        if (keyboard != null && (keyboard.escapeKey.wasPressedThisFrame))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        // if (Cursor.lockState != CursorLockMode.Locked)
        //     return;

        NetworkButtons buttons = default;

        Mouse mouse = Mouse.current;
        if (mouse != null)
        {
            Vector2 mouseDelta = mouse.delta.ReadValue();
            Vector2 lookRotationDelta = new(-mouseDelta.y, mouseDelta.x);
            accumulatedInput.LookDelta += lookRotationDelta;

            // 추가된 부분: 마우스 클릭 감지
            // if (mouse.leftButton.wasPressedThisFrame)
            // {
            //     buttons.Set(InputButton.Interact, true);  // 상호작용 버튼 처리
            // }
            buttons.Set(InputButton.Interact, mouse.leftButton.isPressed);
        }

        if (keyboard != null)
        {
            Vector2 moveDirection = Vector2.zero;

            if (keyboard.wKey.isPressed)
                moveDirection += Vector2.up;
            if (keyboard.sKey.isPressed)
                moveDirection += Vector2.down;
            if (keyboard.aKey.isPressed)
                moveDirection += Vector2.left;
            if (keyboard.dKey.isPressed)
                moveDirection += Vector2.right;
            // if (keyboard.eKey.isPressed)
            // {
            //     buttons.Set(InputButton.Trigger, true);  // E key triggers interaction
            // }
            buttons.Set(InputButton.Trigger, keyboard.eKey.isPressed);
            buttons.Set(InputButton.Qtrigger, keyboard.qKey.isPressed);
            

            buttons.Set(InputButton.RobotUp, keyboard.upArrowKey.isPressed);
            buttons.Set(InputButton.RobotDown, keyboard.downArrowKey.isPressed);
            buttons.Set(InputButton.RobotLeft, keyboard.leftArrowKey.isPressed);
            buttons.Set(InputButton.RobotRight, keyboard.rightArrowKey.isPressed);
            buttons.Set(InputButton.RobotAttach, keyboard.enterKey.isPressed);
            buttons.Set(InputButton.Transform, keyboard.rKey.isPressed);
            
            accumulatedInput.Direction += moveDirection;
            buttons.Set(InputButton.Jump, keyboard.spaceKey.isPressed);
            buttons.Set(InputButton.Run, keyboard.leftShiftKey.isPressed);  // 달리기 버튼 추가

            buttons.Set(InputButton.Zoom, keyboard.mKey.isPressed); // 확대
        }

        accumulatedInput.Buttons = new NetworkButtons(accumulatedInput.Buttons.Bits | buttons.Bits);
    }


    void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner) { }

    void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }

    void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }

    void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }

    void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }

    void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

    void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input)
    {
        accumulatedInput.Direction.Normalize();
        input.Set(accumulatedInput);
        resetInput = true;

        // We have to reset the look delta immediately because we don't want mouse input being reused if another tick is executed during this same frame
        accumulatedInput.LookDelta = default;
    }

    void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

    void INetworkRunnerCallbacks.OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    void INetworkRunnerCallbacks.OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (player == runner.LocalPlayer)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }

    void INetworkRunnerCallbacks.OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }

    void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }

    void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner) { }

    void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner) { }

    void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }

    async void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (shutdownReason == ShutdownReason.DisconnectedByPluginLogic)
        {
            await FindFirstObjectByType<MenuConnectionBehaviour>(FindObjectsInactive.Include).DisconnectAsync(ConnectFailReason.Disconnect);
            FindFirstObjectByType<FusionMenuUIGameplay>(FindObjectsInactive.Include).Controller.Show<FusionMenuUIMain>();
        }
    }

    void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
}
