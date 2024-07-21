using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;
using FusionUtilsEvents;

public class LobbyCanvas : MonoBehaviour
{
    /// <summary>
    /// Fusion에서 제공하는 게임 모드
    /// </summary>
    private GameMode gameMode;

    public string playerNick = "Player";

    public GameLauncher launcher;

    [Space]

    /// <summary>
    /// 플레이어 입장 이벤트 (FusionEvent)
    /// </summary>
    public FusionEvent OnPlayerJoinEvent;

    /// <summary>
    /// 플레이어 퇴장 이벤트 (FusionEvent)
    /// </summary>
    public FusionEvent OnPlayerLeftEvent;

    /// <summary>
    /// 프로그램 종료 이벤트 (FusionEvent)
    /// </summary>
    public FusionEvent OnShutDownEvent;

    // 플레이어 데이터 스폰 이벤트 ??
    public FusionEvent OnPlayerDataSpawnedEvent;
    
    [Space]

    /// <summary>
    /// 시작 화면 UI 게임 오브젝트
    /// </summary>
    public GameObject titleScreen;

    /// <summary>
    /// 생성된 방 UI 게임 오브젝트
    /// </summary>
    public GameObject lobbyScreen;

    public TextMeshProUGUI lobbyPlayerText;
    public TextMeshProUGUI lobbyRoomName;
    public Button startButton;
    public GameObject modeButotns;
    public TMP_InputField nickName;
    public TMP_InputField room;

    private void OnEnable()
    {
        OnPlayerJoinEvent.RegisterResponse(ShowLobbyCanvas);
        OnShutDownEvent.RegisterResponse(ResetCanvas);
        OnPlayerLeftEvent.RegisterResponse(UpdateLobbyList);
        OnPlayerDataSpawnedEvent.RegisterResponse(UpdateLobbyList);
    }

    private void OnDisable()
    {
        OnPlayerJoinEvent.RemoveResponse(ShowLobbyCanvas);
        OnShutDownEvent.RemoveResponse(ResetCanvas);
        OnPlayerLeftEvent.RemoveResponse(UpdateLobbyList);
        OnPlayerDataSpawnedEvent.RemoveResponse(UpdateLobbyList);
    }

    /// <summary>
    /// 게임 모드 설정 함수
    /// </summary>
    /// <param name="gameMode">게임 모드 (solo = 1, Host = 4, Client = 5)</param>
    public void SetGameMode(int gameMode)
    {
        GameManager.instance.SetGameState(GameManager.GameState.Lobby);
        this.gameMode = (GameMode)gameMode; // 게임 모드 저장
        modeButotns.SetActive(false);       // host client 버튼 비활성화
        nickName.transform.parent.gameObject.SetActive(true);        
    }

    /// <summary>
    /// 게임 시작
    /// </summary>
    public void StartLauncher()
    {
        launcher = FindObjectOfType<GameLauncher>();    // 런처 저장
        playerNick = nickName.text;
        PlayerPrefs.SetString("Nick", playerNick);      // 이름 저장
        launcher.Launch(gameMode, room.text);           // 입력된 방 이름과 게임 모드로 세션 시작
        nickName.transform.parent.gameObject.SetActive(false); // 설정창 비활성화
    }

    /// <summary>
    /// 게임 종료
    /// </summary>
    public void ExitGame()
    {
        GameManager.instance.ExitGame();
    }

    public void LeaveLobby()
    {
        // 로비 퇴장
        _ = LeaveLobbyAsync();
    }

    public void StartGame()
    {
        // 게임 시작
        FusionHelper.LocalRunner.SessionInfo.IsOpen = false;
        FusionHelper.LocalRunner.SessionInfo.IsVisible = false;
        // 플레이 씬 연결
        LoadingManager.Instance.LoadPlayLevel(FusionHelper.LocalRunner);
    }

    private async Task LeaveLobbyAsync()
    {
        if(FusionHelper.LocalRunner.IsServer)
        {
            CloseLobby();
        }
        await FusionHelper.LocalRunner?.Shutdown();
    }

    /// <summary>
    /// 로비 종료 함수
    /// </summary>
    public void CloseLobby()
    {
        foreach(var player in FusionHelper.LocalRunner.ActivePlayers)
        {
            // 다른 플레이어 모두 연결 해제
            if(player != FusionHelper.LocalRunner.LocalPlayer)
            {
                FusionHelper.LocalRunner.Disconnect(player);
            }
        }
    }

    // Fusion Event ====================================================================

    /// <summary>
    /// 캔버스 초기화 함수
    /// </summary>
    private void ResetCanvas(PlayerRef player, NetworkRunner runner)
    {
        titleScreen.SetActive(true);
        modeButotns.SetActive(true);
        lobbyScreen.SetActive(false);
        startButton.gameObject.SetActive(runner.IsServer);
    }

    /// <summary>
    /// 로비 스크린 활성화 함수
    /// </summary>
    public void ShowLobbyCanvas(PlayerRef player, NetworkRunner runner)
    {
        titleScreen.SetActive(false);
        lobbyScreen.SetActive(true);
    }

    /// <summary>
    /// 로비 플레이어 리스트 업데이트 함수
    /// </summary>
    /// <param name="playerRef"></param>
    /// <param name="runner"></param>
    public void UpdateLobbyList(PlayerRef playerRef, NetworkRunner runner)
    {
        startButton.gameObject.SetActive(runner.IsServer); // 호스트면 시작 버튼 활성화

        // 플레이어 리스트 문자열 세팅
        string players = default; 
        string isLocal;

        foreach(var player in runner.ActivePlayers)
        {
            isLocal = player == runner.LocalPlayer ? " (You)" : string.Empty; // 로컬 플레이어가 자신이면 you로 표시
            players += GameManager.instance.GetPlayerData(player, runner)?.nickName + isLocal + " \n"; // 플레이어 데이터를 통해 리스트 추가
        }

        lobbyPlayerText.text = players;                             // 플레이어 리스트 텍스트 설정
        lobbyRoomName.text = $"Room : {runner.SessionInfo.Name}";   // 로비 이름 표시
    }
}