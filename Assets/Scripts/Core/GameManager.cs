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

    public GameState state { get; private set; }

}