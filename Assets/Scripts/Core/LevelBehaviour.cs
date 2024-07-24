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

    private Vector3[] spawnPosition = { new(0, 2, 0), new(0, 2, 13), new(13, 2, 0), new(13, 2, 13) };

    // Fusion 함수 ===============================================================================

    // 스폰
    public override void Spawned()
    {
        CreateBoard();
        SpawnPlayer();
    }

    /// <summary>
    /// 스포너에서 플레이어를 스폰하는 함수
    /// </summary>
    private void SpawnPlayer()
    {
        PlayerSpawner spawner = FindObjectOfType<PlayerSpawner>();  // 스포너 가져오기
        spawner.SpawnAllPlayer(FusionHelper.LocalRunner, out List<NetworkObject> list);           // 모든 플레이어 스폰
                                                                                                  
        foreach(var player in list)
        {
            playerObjs.Add(player);

            PlayerBehaviour playerBehaviour = player.GetComponent<PlayerBehaviour>();
            playerBehaviour.InitBeforeSpawn();
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
