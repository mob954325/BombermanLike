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

    // 스폰
    public override void Spawned()
    {
        //CreateBoard();
        SpawnPlayerFromSpawner();

        foreach (var player in playerObjs)
        {
            PlayerBehaviour playerBehaviour = player.GetComponent<PlayerBehaviour>();
            playerBehaviour.InitBeforeSpawn();

        }
    }

    /// <summary>
    /// 스포너에서 플레이어를 스폰하는 함수
    /// </summary>
    private void SpawnPlayerFromSpawner()
    {
        PlayerSpawner spawner = FindObjectOfType<PlayerSpawner>();  // 스포너 가져오기
        spawner.SpawnAllPlayer(FusionHelper.LocalRunner, out List<NetworkObject> list);           // 모든 플레이어 스폰
                                                                                                  
        foreach(var player in list)
        {
            playerObjs.Add(player);
        }
    }

    /// <summary>
    /// 게임 보드 생성 함수
    /// </summary>
    private void CreateBoard()
    {
        Board board = FindObjectOfType<Board>(); // 보드 클래스 가져오기
        board.GenerateBoard(FusionHelper.LocalRunner);
    }

    // 연결해제
    // 승리자 체크
}
