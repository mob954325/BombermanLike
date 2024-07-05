using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Fusion;
using Fusion.Sockets;
using UnityEngine.InputSystem;

public class NetworkSpawner : MonoBehaviour, INetworkRunnerCallbacks
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

    private PlayerInputAction playerInputAction;

    private Vector3 playerInputDir;

    /// <summary>
    /// 플레이어 딕셔너리 (소환된 플레이어 관리용)
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

        InputEnable();
    }

    // 네트워크 콜백 함수 ==================================================

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if(runner.IsServer)
        {
            Vector3 spawnPosition = new Vector3(player.RawEncoded % runner.Config.Simulation.PlayerCount, 0, 0);
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
        NetworkInputData data = new NetworkInputData();

        data.direction = playerInputDir;
        input.Set(data);
    }

    // 유니티 함수 ========================================================

    private void Awake()
    {
        playerInputAction = new PlayerInputAction();
    }

    private void OnDisable()
    {
        InputDisable();
    }

    private void InputEnable()
    {
        playerInputAction.Enable();
        playerInputAction.Player.Move.performed += OnInputMove;
        playerInputAction.Player.Move.canceled += OnInputMove;
    }

    private void InputDisable()
    {
        playerInputAction.Player.Move.canceled -= OnInputMove;
        playerInputAction.Player.Move.performed -= OnInputMove;
        playerInputAction.Enable();
    }

    private void OnInputMove(InputAction.CallbackContext context)
    {
        Vector2 read = context.ReadValue<Vector2>();
        playerInputDir = new Vector3(read.x, 0, read.y);
    }

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