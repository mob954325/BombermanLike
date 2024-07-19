using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FusionUtilsEvents;
using Fusion;

public class GameManager : MonoBehaviour
{
    // 게임 메니저가 해야할 일은?

    // 게임 매니저 싱글톤
    public static GameManager instance;

    /// <summary>
    /// 플레이어 퇴장 이벤트 (FusionEvent)
    /// </summary>
    public FusionEvent OnPlayerLeftEvent;

    /// <summary>
    /// 러너 종료 이벤트 (FusionEvent)??
    /// </summary>
    public FusionEvent OnRunnerShutDownEvent;

    /// <summary>
    /// 플레이어 데이터 딕셔너리 (플레이어 저장용)
    /// </summary>
    private Dictionary<PlayerRef, PlayerData> playerDatas = new Dictionary<PlayerRef, PlayerData>();

    /// <summary>
    /// 게임 상태 enum
    /// </summary>
    public enum GameState
    {
        Lobby = 0,
        Playing,
        Loading
    }

    public GameState State { get; private set; }

    /// <summary>
    /// 나가기 패널 UI (게임 진행중일 때 게임에서 나가기 위한 UI)
    /// </summary>
    private GameObject exitScreen;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    private void OnEnable()
    {
        OnPlayerLeftEvent.RegisterResponse(PlayerDisconnected);
        OnRunnerShutDownEvent.RegisterResponse(DisconnectedFromSession);
    }

    private void OnDisable()
    {
        OnPlayerLeftEvent.RegisterResponse(PlayerDisconnected);
        OnRunnerShutDownEvent.RegisterResponse(DisconnectedFromSession);        
    }

    /// <summary>
    /// 게임 상태 변경 함수
    /// </summary>
    /// <param name="state">게임 상태(0: 로비, 1: 인게임, 2: 로딩)</param>
    public void SetGameState(GameState state)
    {
        State = state;
    }

    /// <summary>
    /// 플레이어 데이터를 찾는 함수
    /// </summary>
    /// <param name="player">플레이어</param>
    /// <param name="runner">러너</param>
    /// <returns></returns>
    public PlayerData GetPlayerData(PlayerRef player, NetworkRunner runner)
    {
        NetworkObject playerObject;
        if(runner.TryGetPlayerObject(player, out playerObject))
        {
            PlayerData data = playerObject.GetComponent<PlayerData>();
            return data;
        }
        else
        {
            Debug.Log("Player not found");
            return null;
        }
    }

    /// <summary>
    /// 모든 플레이어의 인풋을 사용가능하게 하는 함수
    /// </summary>
    public void AllowAllPlayersInputs()
    {
        // 모든 플레이어 인풋 허가
    }

    /// <summary>
    /// 플레이어 데이터 저장 함수
    /// </summary>
    /// <param name="objectInputAuthority">오브젝트 인풋 권한이 있는 playerRef?</param>
    /// <param name="playerData">플레이어 데이터</param>
    public void SetPlayerDataObject(PlayerRef objectInputAuthority, PlayerData playerData)
    {
        playerDatas.Add(objectInputAuthority, playerData);
    }

    /// <summary>
    /// 플레이어 연결 해제 이벤트 함수 (FusionEvent)
    /// </summary>
    /// <param name="player">플레이어</param>
    /// <param name="runner">연결된 러너</param>
    public void PlayerDisconnected(PlayerRef player, NetworkRunner runner)
    {
        runner.Despawn(playerDatas[player].Instance);
        runner.Despawn(playerDatas[player].Object);
        playerDatas.Remove(player);
    }

    public void DisconnectedFromSession(PlayerRef player, NetworkRunner runner)
    {
        Debug.Log("Disconneted from the session");
        ExitSession();
    }

    public void ExitSession()
    {
        SceneManager.LoadScene(0);
        exitScreen.SetActive(false);
    }

    /// <summary>
    /// 게임 종료시 실행하는 함수
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
    }
}