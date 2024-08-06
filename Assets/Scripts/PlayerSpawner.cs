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
    /// 플레이어 스폰 위치
    /// </summary>
    public static Vector3[] spawnPosition;

    /// <summary>
    /// 플레이어 스폰 함수
    /// </summary>
    /// <param name="runner">로컬 러너</param>
    public void SpawnAllPlayer(NetworkRunner runner, out List<NetworkObject> playersList)
    {
        spawnPosition = new Vector3[4];

        playersList = new List<NetworkObject>();
        int index = 0;

        for (int i = 0; i < spawnPosition.Length; i++)
        {
            spawnPosition[i] = transform.GetChild(i).transform.position;
        }

        foreach (var player in runner.ActivePlayers)
        {
            PlayerData data = GameManager.instance.GetPlayerData(player, runner);
            string playerName = data.nickName.ToString();

            if (!runner.IsClient)
            {
                NetworkObject obj = SpawnPlayer(runner, player, spawnPosition[index], playerName);
                data.SetInstance(obj);
            }

            playersList.Add(data.Instance);
            index++;
        }
    }

    /// <summary>
    /// 플레이어 스폰 함수
    /// </summary>
    /// <param name="runner">로컬 네트워크 러너</param>
    /// <param name="player">플레이어</param>
    /// <param name="nick">닉네임</param>
    private NetworkObject SpawnPlayer(NetworkRunner runner, PlayerRef player, Vector3 position, string nick = "")
    {
        NetworkObject playerObject = runner.Spawn
            (
                playerPrefab,
                position,
                Quaternion.identity,
                player
            );

        return playerObject;
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