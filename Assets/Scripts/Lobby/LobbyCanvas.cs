using Fusion;
using FusionUtilsEvents;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LobbyCanvas : MonoBehaviour
{
    /// <summary>
    /// Fusion에서 제공하는 게임 모드
    /// </summary>
    private GameMode gameMode;

    /// <summary>
    /// 플레이어 입장 이벤트 (FusionEvent)
    /// </summary>
    public FusionEvent OnPlayerJoinEvent;

    /// <summary>
    /// 플레이어 퇴장 이벤트 (FusionEvent)
    /// </summary>
    public FusionEvent OnPlayerLeaveEvent;

    /// <summary>
    /// 프로그램 종료 이벤트 (FusionEvent)
    /// </summary>
    public FusionEvent OnShutDownEvent;

    // 플레이어 데이터 스폰 이벤트 ??
    public FusionEvent OnPlayerDataSpawnedEvent;
    
    /// <summary>
    /// 시작 화면 UI 게임 오브젝트
    /// </summary>
    public GameObject titleScreen;

    /// <summary>
    /// 생성된 방 UI 게임 오브젝트
    /// </summary>
    public GameObject lobbyScreen;

    public void ExitGame()
    {
        // 종료
    }

    public void LeaveLobby()
    {
        // 로비 퇴장
    }

    public void StartGame()
    {
        // 게임 시작
    }

/*    
 *   로비 퇴장 async 함수
 *    
    private async Task LeaveLobbyEvent()
    {
        await _;
    }
*/


    // Fusion Event ====================================================================

    /// <summary>
    /// 캔버스 리셋 이벤트
    /// </summary>
    private void ResetCanvas(PlayerPrefs player, NetworkRunner runner)
    {

    }

    /// <summary>
    /// 로비 UI 활성화 이벤트
    /// </summary>
    private void ShowLobbyCanvas(PlayerPrefs player, NetworkRunner runner)
    {

    }

    /// <summary>
    /// 로비 업데이트 이벤트 (추가된 플레이어 보여주기)
    /// </summary>
    private void UpdateLobbyList(PlayerPrefs player, NetworkRunner runner)
    {

    }
}