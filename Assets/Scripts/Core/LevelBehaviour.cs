using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

/// <summary>
/// 플레이 씬 관리 클래스 
/// </summary>
public class LevelBehaviour : NetworkBehaviour
{
    public List<NetworkObject> playerObjs = new List<NetworkObject>();

    // Fusion 함수 ===============================================================================

    public override void Spawned()
    {
        CreateBoard();
        SpawnPlayer();
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

    private void CreateBoard()
    {
        Board board = FindObjectOfType<Board>();
        board.Init(FusionHelper.LocalRunner);
    }

    // 연결해제
    // 승리자 체크
}
