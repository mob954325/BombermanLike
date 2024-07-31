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

    public EffectManager effectManager;

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
    public void Init(Vector2Int spawnPosition, PlayerBehaviour player, int length)
    {
        gameObject.name = $"Bomb_{Id}";
        explosionLength = length;

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
        effectManager.ClearParticles();
    }

    public override void FixedUpdateNetwork()
    {
        if (bombTimer.Expired(Runner))
        {
            OnExplosion();
            Runner.Despawn(Object);        
        }        
    }

    /// <summary>
    /// 폭탄 폭발 이팩트 함수 (이팩트 매니저를 위해서 플레이어에 작성)
    /// </summary>
    /// <param name="position">폭발하는 위치</param>
    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    public void RPC_ExplosionEffect(Vector3 position)
    {
        effectManager.PlayParticle((int)EffectType.Explosion, CoordinateConversion.GetGridCenter(position, Board.CellSize));
    }

    public void MultipleExplosionEffects(List<Vector2Int> list)
    {
        foreach (var item in list)
        {
            Vector3 world = CoordinateConversion.GridToWorld(item);
            RPC_ExplosionEffect(world);
        }
    }

    // 기능 함수 ===============================================================================

    private void OnExplosion()
    {
        if(Runner.IsServer)
        {
            List<Vector2Int> positions = GetExplosionPosition(); // 폭발 위치        
            levelBehaviour.CheckHitPlayers(positions);
            levelBehaviour.CheckHitCells(positions);
            MultipleExplosionEffects(positions);
        }
    }

    /// <summary>
    /// 폭발 위치 반환 함수
    /// </summary>
    /// <returns>그리드값이 저장된 Vector2Int형 리스트</returns>
    private List<Vector2Int> GetExplosionPosition()
    {
        // 방향 배열
        int[] dx = { 1, -1, 0, 0 };
        int[] dy = { 0, 0, -1, 1 };

        // 부서진 셀 개수 확인 (관통 파괴 방지용)
        int[] checkBreakedCellCount = { 0, 0, 0, 0 }; // (위, 아래, 왼쪽, 오른쪽)

        List<Vector2Int> result = new List<Vector2Int>
        {
            spawnGrid // 현재 폭탄 위치
        };

        for(int i = 1; i < explosionLength + 1; i++)
        {
            for(int d = 0; d < 4; d++)
            {
                if (checkBreakedCellCount[d] > 0)   // 방향에서 부서진 셀이 있으면 무시
                   continue;

                Vector2Int nextGrid = new Vector2Int(spawnGrid.x + dx[d] * i, spawnGrid.y + dy[d] * i);

                if (levelBehaviour.IsExistCell(nextGrid, out CellType type)) // 해당 그리드에 셀이 존재하면
                {
                    checkBreakedCellCount[d]++; // 파괴 개수 추가
                }

                if(IsVaildPosition(nextGrid)) // 해당 위치가 존재하면
                {
                    result.Add(nextGrid); // 리스트 추가
                }
            }
        }

        return result;
    }

    /// <summary>
    /// 폭발하는 위치가 보드 안인지 확인하는 함수
    /// </summary>
    /// <returns>보드 안이면 true 밖이면 false</returns>
    private bool IsVaildPosition(Vector2Int grid)
    {
        return grid.x > -1 && grid.x < Board.BoardSize && grid.y > -1 && grid.y < Board.BoardSize;
    }
}