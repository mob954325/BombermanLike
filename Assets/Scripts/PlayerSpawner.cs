using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Fusion;
using Fusion.Sockets;

/// <summary>
/// 플레이어 스폰관련 클래스
/// </summary>
public class PlayerSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    /// <summary>
    /// 네트워크 러너
    /// </summary>
    private NetworkRunner runner;

    /// <summary>
    /// 플레이어 프리팹
    /// </summary>
    [SerializeField]
    private NetworkPrefabRef playerPrefab;

    [SerializeField]
    private NetworkPrefabRef boardPrefab;

    /// <summary>
    /// 플레이어 딕셔너리 (생성된 플레이어 관리용)
    /// </summary>
    private Dictionary<PlayerRef, NetworkObject> spawnedPlayers = new Dictionary<PlayerRef, NetworkObject>();

    // 기능 함수 =========================================================
    async void StartGame(GameMode mode)
    {
        runner = gameObject.AddComponent<NetworkRunner>();
        runner.ProvideInput = true; // 입력 데이터 제공

        // 현재 씬의 네트워크 씬 인포 추가
        SceneRef scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        NetworkSceneInfo sceneInfo = new NetworkSceneInfo();

        if(scene.IsValid)
        {   // 씬 레퍼런스 추가
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        // 세션에 시작 or 참가
        await runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "TestSpawnScene",
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });

        CreateBoard();
    }
    
    public void CreateBoard()
    {
        if(runner.IsServer)
        {
            NetworkObject networkBoardObject = runner.Spawn(boardPrefab, Vector3.zero, Quaternion.identity, null);    // 보드 생성
            Board board = networkBoardObject.GetComponent<Board>(); // 보드 컴포넌트 접근
            board.GenerateBoard();
        }
    }

    // 네트워크 콜백 함수 ==================================================

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if(runner.IsServer)
        {
            Vector3 spawnPosition = new Vector3(0.5f, 1, 0.5f);
            NetworkObject networkPlayerObject = runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, player);

            spawnedPlayers.Add(player, networkPlayerObject);
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if(spawnedPlayers.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);  // 플레이어 디스폰
            spawnedPlayers.Remove(player);  // 리스트 제거
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {

    }

    // 유니티 함수 ========================================================

    // 게임 시작될 때 생성될 GUI(게임 모드에 따른 참가)
    private void OnGUI()
    {
        if (runner == null)
        {
            if (GUI.Button(new Rect(0, 0, 200, 40), "Host"))
            {
                StartGame(GameMode.Host);
            }
            if (GUI.Button(new Rect(0, 40, 200, 40), "Join"))
            {
                StartGame(GameMode.Client);
            }
        }
    }

    // 사용 안함 ==========================================================
    public void OnConnectedToServer(NetworkRunner runner)
    {
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }
}