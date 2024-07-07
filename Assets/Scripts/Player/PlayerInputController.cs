using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using UnityEngine.InputSystem;

public class PlayerInputController : NetworkBehaviour, INetworkRunnerCallbacks
{
    PlayerInputAction playerInputAction;

    /// <summary>
    /// 플레이어 방향 값
    /// </summary>
    private Vector3 playerInputDir;

    private void Awake()
    {
        playerInputAction = new PlayerInputAction();
    }

    private void OnDisable()
    {
        OnPlayerEnable();
    }

    private void OnPlayerDisable()
    {
        playerInputAction.Player.Enable();
        playerInputAction.Player.Move.performed += OnMoveInput;
        playerInputAction.Player.Move.canceled += OnMoveInput;
    }


    private void OnPlayerEnable()
    {
        playerInputAction.Player.Move.canceled -= OnMoveInput;
        playerInputAction.Player.Move.performed -= OnMoveInput;
        playerInputAction.Player.Disable();
    }

    private void OnMoveInput(InputAction.CallbackContext context)
    {
        Vector2 inputVec = context.ReadValue<Vector2>();
        playerInputDir = new Vector3(inputVec.x, 0, inputVec.y);
    }

    public override void Spawned() // 스폰 되었을 때 실행
    {
        if(Object.HasInputAuthority)
        {
            Runner.AddCallbacks(this);
            OnPlayerDisable();
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input) // 인풋 받는 함수
    {
        PlayerInputData inputData = new PlayerInputData();

        inputData.direction = playerInputDir;
        input.Set(inputData);
    }

    #region Not used CallBacks
    public void OnConnectedToServer(NetworkRunner runner) { }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }

    public void OnSceneLoadDone(NetworkRunner runner) { }

    public void OnSceneLoadStart(NetworkRunner runner) { }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    #endregion
}