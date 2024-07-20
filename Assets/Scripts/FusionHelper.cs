using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using FusionUtilsEvents;

/// <summary>
/// Fusion의 INetworkRunnerCallbacks를 구현한 클래스
/// </summary>
public class FusionHelper : MonoBehaviour, INetworkRunnerCallbacks
{
    public static NetworkRunner LocalRunner;

    public NetworkPrefabRef PlayerDataObject;

    public FusionEvent OnPlayerJoinedEvent;
    public FusionEvent OnPlayerLeftEvent;
    public FusionEvent OnshutdownEvent;
    public FusionEvent OnDisconnectEvent;

    // 사용 함수 ==========================================================================================================================

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) 
    { 
        if(runner.IsServer)
        {
            runner.Spawn(PlayerDataObject, inputAuthority: player); // ?
        }

        if(runner.LocalPlayer == player)
        {
            LocalRunner = runner;
        }

        OnPlayerJoinedEvent?.ExecuteResponses(player, runner);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) 
    {
        OnPlayerLeftEvent?.ExecuteResponses(player, runner);
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) 
    {
        OnPlayerLeftEvent?.ExecuteResponses(runner: runner);
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) 
    {
        OnPlayerLeftEvent?.ExecuteResponses(runner: runner);
    }

    // 비사용 함수 ===========================================================================================================================
    public void OnConnectedToServer(NetworkRunner runner) { }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) {}

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) {}

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) {}

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) {}

    public void OnInput(NetworkRunner runner, NetworkInput input) {}

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) {}

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) {}

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) {}

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) {}

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) {}

    public void OnSceneLoadDone(NetworkRunner runner) {}

    public void OnSceneLoadStart(NetworkRunner runner) {}

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) {}

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) {}
}