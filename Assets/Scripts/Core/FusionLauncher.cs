using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;

public class FusionLauncher : MonoBehaviour
{
    private NetworkRunner runner;
    private ConnectionStatus status;

    public enum ConnectionStatus
    {
        Disconnected,
        Connecting,
        Failed,
        Connected,
        Loading,
        Loaded
    }

    /// <summary>
    /// 실행 비동기 함수
    /// </summary>
    /// <param name="mode">모드</param>
    /// <param name="room">방 이름</param>
    /// <param name="sceneLoader">퓨전 네트워크 씬 매니저?</param>
    public async void Launch(GameMode mode, string room, INetworkSceneManager sceneLoader)
    {
        SetConnectionStatus(ConnectionStatus.Connecting, "");
        DontDestroyOnLoad(gameObject);

        if(runner == null)
        {
            runner = gameObject.AddComponent<NetworkRunner>();  // 넷러너 추가`
            runner.name = name;
            runner.ProvideInput = mode != GameMode.Server; // 서버가 아니면 인풋 정보 수집?
        }

        // 러너 게임 시작
        await runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = room,
            SceneManager = sceneLoader
        });
    }

    /// <summary>
    /// 연결 상태 설정 함수
    /// </summary>
    /// <param name="status">현재 연결 상태</param>
    public void SetConnectionStatus(ConnectionStatus status, string message)
    {
        this.status = status;
    }
}
