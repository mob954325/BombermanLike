using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

/// <summary>
/// 플레이 씬 관리 클래스 
/// </summary>
public class LevelBehaviour : NetworkBehaviour
{
    /// <summary>
    /// 폭탄 업그레이드 프리팹
    /// </summary>
    public NetworkPrefabRef bombUpgradeItem;

    public List<NetworkObject> playerObjs = new List<NetworkObject>();
    public List<Cell> cells = new List<Cell>();

    private PlayerInfoPanel playerInfoPanel;
    private TimerPanel timerPanel;
    private EndGamePanel endGamePanel;

    /// <summary>
    /// 타이머가 끝났을 때 실행하는 델리게이트
    /// </summary>
    public Action OnTimerEnd;

    /// <summary>
    /// 생존 플레이어 수
    /// </summary>
    [SerializeField]private int alivePlayerCount;

    private bool isPlaying = false;

    private void Awake()
    {
        playerInfoPanel = FindAnyObjectByType<PlayerInfoPanel>();
        timerPanel = FindAnyObjectByType<TimerPanel>();
        endGamePanel = FindAnyObjectByType<EndGamePanel>(FindObjectsInactive.Include);
    }

    // Fusion 함수 ===============================================================================

    public override void Spawned()
    {
        CreateBoard();
        SpawnPlayer();
        SetUI();

        isPlaying = true;
    }

    public override void FixedUpdateNetwork()
    {
        if(timerPanel.GetTimeValue() < 1)
        {
            TimeOut_FinishGame();
        }
    }

    // 기능 함수 ===============================================================================

    /// <summary>
    /// 스포너에서 플레이어를 스폰하는 함수
    /// </summary>
    private void SpawnPlayer()
    {
        PlayerSpawner spawner = FindObjectOfType<PlayerSpawner>();  // 스포너 가져오기
        spawner.SpawnAllPlayer(FusionHelper.LocalRunner, out List <NetworkObject> list);           // 모든 플레이어 스폰

        int index = 0;

        foreach(var player in list)
        {
            playerObjs.Add(player);

            PlayerBehaviour playerBehaviour = player.GetComponent<PlayerBehaviour>();
            playerBehaviour.InitBeforeSpawn(index);
            index++;
            alivePlayerCount++;
        }

        Debug.Log(alivePlayerCount);
    }
    
    /// <summary>
    /// 플레이어가 폭발에 맞는지 확인하는 함수
    /// </summary>
    /// <param name="grids">폭발 위치 그리드값 리스트</param>
    public void CheckHitPlayers(List<Vector2Int> grids)
    {
        foreach(var obj in playerObjs)
        {
            PlayerBehaviour player = obj.GetComponent<PlayerBehaviour>();

            foreach(var grid in grids)
            {
                if(grid == player.GetGridPosition())
                {
                    player.RPC_OnHit();
                }
            }
        }
    }

    /// <summary>
    /// 셀이 폭발에 맞는지 확인하는 함수
    /// </summary>
    /// <param name="grids">폭발 위치 그리드값 리스트</param>
    public void CheckHitCells(List<Vector2Int> grids)
    {
        foreach (var obj in cells)
        {
            Cell cell = obj.GetComponent<Cell>();

            foreach (var grid in grids)
            {
                if (grid == cell.GetGridPosition()
                    && cell.GetCellType() == CellType.Breakable
                    && cell.gameObject.activeSelf)
                {
                    cell.RPC_OnHit();
                    SpawnItem(FusionHelper.LocalRunner, CoordinateConversion.GetGridCenter(grid, Board.CellSize));
                }
            }
        }
    }

    /// <summary>
    /// 그리드 위치에 셀이 존재하는 지 확인하는 함수
    /// </summary>
    /// <param name="grid">그리드</param>
    /// <param name="type">셀 타입</param>
    /// <returns>존재하면 true 아니면 false</returns>
    public bool IsExistCell(Vector2Int grid, out CellType type)
    {
        bool result = false;
        type = CellType.Floor;

        foreach(var cell in cells)
        {
            Vector2Int cellGrid = cell.GetGridPosition();

            if(grid == cellGrid && cell.GetComponent<NetworkBehaviour>().isActiveAndEnabled)
            {
                result = true;
                break;
            }

            type = cell.GetCellType();
        }

        return result;
    }

    /// <summary>
    /// 보드 생성 함수
    /// </summary>
    private void CreateBoard()
    {
        Board board = FindObjectOfType<Board>();
        board.Init(FusionHelper.LocalRunner, out List<Cell> list);

        int index = 0;
        foreach(var cell in list)
        {
            cells.Add(cell);
            index++;
        }
    }

    private void SetUI()
    {
        playerInfoPanel.Init(FusionHelper.LocalRunner);
    }

    /// <summary>
    /// 랜덤으로 업그레이드 아이템 생성하는 함수
    /// </summary>
    private void SpawnItem(NetworkRunner runner, Vector3 world)
    {
        if(runner.IsServer)
        {
            FusionHelper.LocalRunner.Spawn(bombUpgradeItem, world, Quaternion.identity);
        }
    }

    /// <summary>
    /// 플레이어 죽을 때 실행되는 함수
    /// </summary>
    public void OnPlayerDie()
    {
        PlayerData data = null;
        alivePlayerCount--;

        if(alivePlayerCount <= 1)
        {
            foreach(var player in FusionHelper.LocalRunner.ActivePlayers)
            {
                PlayerData curData = GameManager.instance.GetPlayerData(player, FusionHelper.LocalRunner);
                PlayerBehaviour playerBehaviour = curData.Instance.GetComponent<PlayerBehaviour>(); 
                if(playerBehaviour.Hp > 0) // 생존한 플레이어 데이터 저장
                {
                    data = curData;
                }
            }

            LastStanding_FinishGame(data);
        }
    }

    /// <summary>
    /// 게임이 끝나면 실행되는 함수
    /// </summary>
    private void LastStanding_FinishGame(PlayerData data)
    {
        if (!isPlaying)
            return;

        isPlaying = false;

        endGamePanel.ShowPanel(data);
    }


    private void TimeOut_FinishGame()
    {
        if (!isPlaying)
            return;

        isPlaying = false;
        endGamePanel.ShowPanel(null);
    }
}

