using System.Collections;
using System.Threading.Tasks;
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
    /// 레벨 매니저
    /// </summary>
    public LevelManager levelManager;


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
    [SerializeField] private GameObject exitScreen;

    [Space]

    /// <summary>
    /// 플레이어 퇴장 이벤트 (FusionEvent)
    /// </summary>
    public FusionEvent OnPlayerLeftEvent;

    /// <summary>
    /// 러너 종료 이벤트 (FusionEvent)??
    /// </summary>
    public FusionEvent OnRunnerShutDownEvent;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(this.transform.parent.gameObject);
        }

        DontDestroyOnLoad(transform.parent);
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
        foreach(PlayerBehaviour behaviour in FindObjectsOfType<PlayerBehaviour>())
        {
            behaviour.SetInputsAllowed(true);
        }
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

    /// <summary>
    /// 세션에서 연결 해제하는 함수
    /// </summary>
    /// <param name="player">플레이어</param>
    /// <param name="runner">러너</param>
    public void DisconnectedFromSession(PlayerRef player, NetworkRunner runner)
    {
        Debug.Log("Disconneted from the session");
        ExitSession();
    }
    
    public void LeaveRoom()
    {
        _ = LeaveRoomAsync();
    }
    
    private async Task LeaveRoomAsync()
    {
        await ShutDownRunner();
    }

    private async Task ShutDownRunner()
    {
        // 셧다운 내용
        await FusionHelper.LocalRunner?.Shutdown();
        SetGameState(GameState.Lobby);
        playerDatas.Clear();
    }

    /// <summary>
    /// 세션 퇴장 함수
    /// </summary>
    public void ExitSession()
    {
        _ = ShutDownRunner();
        levelManager.ResetLoadedScene();
        SceneManager.LoadScene(0);
        exitScreen.SetActive(false);
    }

    /// <summary>
    /// 게임 종료시 실행하는 함수
    /// </summary>
    public void ExitGame()
    {
        _ = ShutDownRunner();
        Application.Quit();
    }

    /// <summary>
    /// 종료 화면 켜져있는지 확인하는 함수
    /// </summary>
    /// <returns>켜저있으면 true 아니면 false</returns>
    public bool IsExitScreenOpen()
    {
        return exitScreen.activeSelf;
    }

    /// <summary>
    /// 종료 캠버스 활성화 함수
    /// </summary>
    public void ShowExitScreen()
    {
        exitScreen.SetActive(true);
    }

    /// <summary>
    /// 종료 캠버스 비활성화 함수
    /// </summary>
    public void CloseExitScreen()
    { 
        exitScreen.SetActive(false);
    }
}