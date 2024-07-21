using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

/// <summary>
/// 플레이 씬 관리 클래스 
/// </summary>
public class LevelBehaviour : NetworkBehaviour
{
    // 스폰
    public override void Spawned()
    {
        // 스폰 == 씬 변경
        PlayerSpawner spawner = FindObjectOfType<PlayerSpawner>();  // 스포너 가져오기
        spawner.SpawnAllPlayer(FusionHelper.LocalRunner);           // 모든 플레이어 스폰
        GameStart();                                                // 게임 시작
    }

    /// <summary>
    /// 게임 시작 시 실행되는 함수
    /// </summary>
    private void GameStart()
    {

    }

    // 연결해제
    // 승리자 체크
}
