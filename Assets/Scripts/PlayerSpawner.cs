using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Fusion;
using Fusion.Sockets;
using Random = UnityEngine.Random;

/// <summary>
/// 플레이어 스폰 담당 클래스
/// </summary>
public class PlayerSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    /// <summary>
    /// 플레이어 프리팹
    /// </summary>
    [SerializeField]
    private NetworkPrefabRef playerPrefab;

    [SerializeField]
    private NetworkPrefabRef boardPrefab;

    /// <summary>
    /// 스폰 위치
    /// </summary>
    private Vector3 spawnPosition = Vector3.zero;
    
    public void SpawnAllPlayer(NetworkRunner runner)
    {
        if(!runner.IsClient) // Host만 처리
        {
            foreach(var player in runner.ActivePlayers)
            {
                string playerName = GameManager.instance.GetPlayerData(player, runner).nickName.ToString();
                SpawnPlayer(runner, player, playerName);
            }
        }
    }

    /// <summary>
    /// 플레이어 스폰 함수
    /// </summary>
    /// <param name="runner">로컬 네트워크 러너</param>
    /// <param name="player">플레이어</param>
    /// <param name="nick">닉네임</param>
    private void SpawnPlayer(NetworkRunner runner, PlayerRef player, string nick = "")
    {
        NetworkObject playerObject = runner.Spawn
            (
                playerPrefab,
                spawnPosition,
                Quaternion.identity,
                player,
                SpawnInit
            );
    }

    /// <summary>
    /// 스폰 초기화 함수
    /// </summary>
    private void SpawnInit(NetworkRunner runner, NetworkObject obj)
    {
        PlayerBehaviour playerBehaviour = obj.GetComponent<PlayerBehaviour>();
        playerBehaviour.Init(runner.LocalPlayer.PlayerId, SetColor());
    }

    private Color SetColor()
    {
        return new Color
            (
                Random.value,
                Random.value,
                Random.value
            );
    }

    #region Unused CallBacks
    public void OnConnectedToServer(NetworkRunner runner) { }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) {}

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) {}

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) {}

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) {}

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) {}

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) {}

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) {}

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) {}

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) {}

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) {}

    public void OnSceneLoadDone(NetworkRunner runner) {}

    public void OnSceneLoadStart(NetworkRunner runner) {}

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) {}

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) {}

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) {}

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) {}

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) {}

    public void OnInput(NetworkRunner runner, NetworkInput input) {}
    #endregion
}