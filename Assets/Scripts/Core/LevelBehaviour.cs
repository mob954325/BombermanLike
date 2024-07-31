using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

/// <summary>
/// 플레이 씬 관리 클래스 
/// </summary>
public class LevelBehaviour : NetworkBehaviour
{
    public NetworkPrefabRef bombUpgradeItem;

    public List<NetworkObject> playerObjs = new List<NetworkObject>();
    public List<Cell> cells = new List<Cell>();

    // Fusion 함수 ===============================================================================

    public override void Spawned()
    {
        CreateBoard();
        SpawnPlayer();

        //FusionHelper.LocalRunner.Spawn(testitem, Vector3.zero + Vector3.up * 3f, Quaternion.identity);
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
        }
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
                    && cell.GetCellType() == CellType.Breakable)
                {
                    cell.RPC_OnHit();
                    RPC_SpawnItem(CoordinateConversion.GetGridCenter(grid, Board.CellSize));
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

    /// <summary>
    /// 랜덤으로 업그레이드 아이템 생성하는 함수
    /// </summary>
    [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
    private void RPC_SpawnItem(Vector3 world)
    {
        FusionHelper.LocalRunner.Spawn(bombUpgradeItem, world, Quaternion.identity);
    }

    // 연결해제
    // 승리자 체크
}
