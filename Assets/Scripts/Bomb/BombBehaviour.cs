using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class BombBehaviour : NetworkBehaviour 
{
    // 타이머
    // 타이머가 끝나면 십자가 모양으로 터짐
    // 이펙트 생성
    // 오브젝트 디스폰

    /// <summary>
    /// 소환한 플레이어
    /// </summary>
    private PlayerBehaviour player;

    /// <summary>
    /// 레벨 매니저
    /// </summary>
    private LevelBehaviour levelBehaviour;

    /// <summary>
    /// 스폰 위치 그리드값
    /// </summary>
    [SerializeField] private Vector2Int spawnGrid = Vector2Int.zero;

    /// <summary>
    /// 터지는 시간 타이머
    /// </summary>
    [Networked]
    private TickTimer bombTimer { get; set; }

    /// <summary>
    /// 터지는데 걸리는 시간
    /// </summary>
    public float timeToExplosion = 3f;

    /// <summary>
    /// 폭발 길이 (Default : 1)
    /// </summary>
    [Networked]
    private int explosionLength { get; set; }

    /// <summary>
    /// 폭탄 초기화 함수
    /// </summary>
    /// <param name="spawnPosition">스폰한 그리드 위치 값</param>
    public void Init(Vector2Int spawnPosition, PlayerBehaviour player)
    {
        gameObject.name = $"Bomb_{Id}";
        explosionLength = 1;

        spawnGrid = spawnPosition;
        this.player = player;

        levelBehaviour = FindAnyObjectByType<LevelBehaviour>(); 
    }

    // 네트워크 함수 ===============================================================================

    public override void Spawned()
    {
        if (Object.HasStateAuthority == false)
            return;

        bombTimer = TickTimer.CreateFromSeconds(Runner, timeToExplosion);
    }

    public override void FixedUpdateNetwork()
    {
        if (bombTimer.Expired(Runner))
        {
            OnExplosion();
            Runner.Despawn(Object);        
        }        
    }

    // 기능 함수 ===============================================================================

    private void OnExplosion()
    {
        if(Runner.IsServer)
        {
            player.RPC_ExplosionEffect(this.transform.position);
            List<Vector2Int> positions = GetExplosionPosition(); // 폭발 위치        
            levelBehaviour.CheckHitPlayers(positions);
            levelBehaviour.CheckHitCells(positions);
        }
    }

    /// <summary>
    /// 폭발 위치 반환 함수
    /// </summary>
    /// <returns>그리드값이 저장된 Vector2Int형 리스트</returns>
    private List<Vector2Int> GetExplosionPosition()
    {
        List<Vector2Int> result = new List<Vector2Int>
        {
            spawnGrid // 현재 폭탄 위치
        };

        for(int i = 1; i < explosionLength + 1; i++)
        {
            result.Add(spawnGrid + Vector2Int.up * i);
            result.Add(spawnGrid + Vector2Int.down * i);
            result.Add(spawnGrid + Vector2Int.left * i);
            result.Add(spawnGrid + Vector2Int.right * i);
        }

        return result;
    }

    /// <summary>
    /// 폭발 길이 추가하는 함수
    /// </summary>
    public void IncreaseExplosionLength()
    {
        explosionLength++;
    }
}