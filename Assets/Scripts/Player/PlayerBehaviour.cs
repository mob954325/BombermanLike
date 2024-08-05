using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Random = UnityEngine.Random;

/// <summary>
/// 플레이어 행동 클래스
/// </summary>
public class PlayerBehaviour : NetworkBehaviour, IHealth
{
    /// <summary>
    /// 플레이어 데이터
    /// </summary>
    private PlayerData data;

    /// <summary>
    /// 파티클 매니저
    /// </summary>
    public EffectManager effectManager;

    /// <summary>
    /// 폭탄 프리팹
    /// </summary>
    public BombBehaviour bombPrefab;

    /// <summary>
    /// 플레이어 모델 머터리얼
    /// </summary>
    [SerializeField] private Material[] playerMaterials;
    //bodyMaterial

    private ChangeDetector changeDetector;

    /// <summary>
    /// 플레이어 콜라이더
    /// </summary>
    private Collider collider;

    /// <summary>
    /// 부딪힌 콜라이더
    /// </summary>
    private Collider[] hitColliders;

    /// <summary>
    /// 플레이어 현재 그리드 위치값
    /// </summary>
    [Networked]
    private Vector2Int CurrentGrid { get; set; }

    /// <summary>
    /// 오브젝트 색깔
    /// </summary>
    [Networked]
    public Color objColor { get; set; }

    /// <summary>
    /// 플레이어 폭탄의 폭발 길이
    /// </summary>
    private int explosionLength = 1;

    /// <summary>
    /// 입력 허가 체크 변수
    /// </summary>
    private bool InputsAllowed;

    /// <summary>
    /// 스폰 확인 여부(초기화 후 true)
    /// </summary>
    [Networked]
    private bool isSpawed { get; set; } = false;

    /// <summary>
    /// 폭탄 설치했는지 확인하는 변수 (폭탄 여러개 설치 방지용)
    /// </summary>
    private bool isSetBomb = false;

    /// <summary>
    /// 몇번째 스폰인지 저장하는 변수
    /// </summary>
    public int No;

    public int Hp 
    { 
        get => hp;
        set
        {
            hp = value;

            if(hp < 1) // 사망 처리
            {
                hp = 0;
                RPC_OnDie();
            }
        }
    }

    /// <summary>
    /// 현재 체력
    /// </summary>
    private int hp;

    public int MaxHp { get => maxHp; set => maxHp = value; }

    /// <summary>
    /// 최대 체력
    /// </summary>
    private int maxHp = 3;

    /// <summary>
    /// 플레이어가 공격을 맞을 때 호출되는 델리게이트
    /// </summary>
    public Action OnHit;

    /// <summary>
    /// 플레이어 사망시 호출되는 델리게이트
    /// </summary>
    public Action OnDie;

    // 유니티 함수 ===============================================================================

    private void Awake()
    {
        playerMaterials = new Material[4];

        Transform child = transform.GetChild(0);
        for(int i = 0; i < playerMaterials.Length; i++)
        {
            playerMaterials[i] = child.GetChild(0).GetChild(i).GetComponent<SkinnedMeshRenderer>().material;
        }
        collider = transform.GetChild(1).GetComponent<Collider>();

        hitColliders = new Collider[2];
    }

    // Fusion 함수 ===============================================================================

    public override void Spawned()
    {
        changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        effectManager.ClearParticles();

        maxHp = 3;
        hp = maxHp;

        OnDie += FindAnyObjectByType<LevelBehaviour>().OnPlayerDie;
    }

    public override void Render()
    {
        DetectCollsion();

        foreach (var change in changeDetector.DetectChanges(this))
        {
            switch(change)
            {
                case nameof(isSpawed):
                    // 색상 변경
                    ChangeMaterialsColor(Color.white, objColor, true);
                    break;
            }
        }

        ChangeMaterialsColor(Color.white, objColor, true);
    }

    // 기능 함수 =================================================================================

    /// <summary>
    /// 폭탄 설치하는 함수
    /// </summary>
    public void SetBomb(Vector3 worldGridPosition)
    {
        if (isSetBomb)
            return;

        Runner.Spawn(bombPrefab,
            worldGridPosition,
            Quaternion.identity,
            Object.InputAuthority,
            (runner, o) =>
            {
                o.GetComponent<BombBehaviour>().Init(CurrentGrid, this, explosionLength);
            });

        isSetBomb = true;
    }

    /// <summary>
    /// 폭탄 터지고 실행되는 함수 (폭탄 설치 혀용하게 만들기)
    /// </summary>
    public void OnBombExplosioned()
    {
        isSetBomb = false;
    }

    /// <summary>
    /// 인풋 입력 허가 설정 함수
    /// </summary>
    /// <param name="value">true : 허가 , false 비허가</param>
    public void SetInputsAllowed(bool value)
    {
        InputsAllowed = value;
    }

    /// <summary>
    /// 스폰 여부(isSpawed) true로 변경하는 함수
    /// </summary>
    public void SetIsSpawnedTrue()
    {
        isSpawed = true;
    }

    /// <summary>
    /// 스폰전 초기화 함수
    /// </summary>
    public void InitBeforeSpawn(int index)
    {
        objColor = new Color(Random.value, Random.value, Random.value);
        No = index;

        isSpawed = true;
    }

    /// <summary>
    /// 그리드 위치를 설정하는 함수
    /// </summary>
    public void SetGridPosition(Vector3 worldPosition)
    {
        CurrentGrid = CoordinateConversion.WorldToGrid(worldPosition + Vector3.one * 0.1f);
    }

    public Vector2Int GetGridPosition()
    {
        return CurrentGrid;
    }

    /// <summary>
    /// 피격 받을 때 색상 변경 코루틴
    /// </summary>
    /// <param name="material">바꿀 머터리얼</param>
    /// <param name="prev">이전 색상(플레이어의 원래 색상)</param>
    /// <param name="next">바꿀 색상(머터리얼이 가질 색상)</param>
    private IEnumerator ChangeMaterialColor(Material material, Color prev, Color next)
    {
        Debug.Log("asfd");
        float remainTime = 1f;

        while (remainTime > 0f)
        {
            remainTime -= Time.deltaTime;
            material.color = Color.Lerp(next, prev, remainTime);

            yield return null;
        }
    }

    /// <summary>
    /// 폭발 길이 추가하는 함수
    /// </summary>
    public void IncreaseExplosionLength()
    {
        explosionLength++;
    }

    private void DetectCollsion()
    {
        int hitCount = FusionHelper.LocalRunner.GetPhysicsScene().OverlapBox(
            transform.position,
            collider.bounds.size * 0.9f,
            hitColliders,
            Quaternion.identity,
            LayerMask.GetMask("Item"),
            QueryTriggerInteraction.UseGlobal);

        if(hitColliders != default) // 아이템 레이어를 가진 오브젝트에 충돌했을 때
        {
            foreach(var coll in hitColliders) // 충돌 확인
            {
                if(coll == null)
                    continue;

                if(coll.tag.Equals("Item") && coll.gameObject.activeSelf) // 아이템 태그 확인
                {
                    ItemObject item = coll.GetComponentInParent<ItemObject>();
                    item.OnPick();
                    IncreaseExplosionLength();
                    break;
                }
            }
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

    public void SetData(PlayerData data)
    {
        this.data = data;
    }

    /// <summary>
    /// 플레이어 데이터
    /// </summary>
    /// <returns></returns>
    public PlayerData GetData()
    {
        return data;
    }

    /// <summary>
    /// 플레이어의 모든 머터리얼 색 변경 
    /// </summary>
    /// <param name="prev">원래 색상</param>
    /// <param name="next">바꿀 색상</param>
    /// <param name="isImmediately">즉시 변경하면 true 아니면 false</param>
    private void ChangeMaterialsColor(Color prev, Color next, bool isImmediately = false)
    {
        if(!isImmediately)
        {
            for(int i = 0; i < playerMaterials.Length; i++)
            {
               StartCoroutine(ChangeMaterialColor(playerMaterials[i], prev, next));
            }
        }
        else
        {
            for (int i = 0; i < playerMaterials.Length; i++)
            {
                playerMaterials[i].color = next;
            }
        }
    }

    // IHealth =================================================================================

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    public void RPC_OnHit()
    {
        Hp--;

        OnHit?.Invoke();
        ChangeMaterialsColor(objColor, Color.red, true);
        ChangeMaterialsColor(Color.red, objColor);
    }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    public void RPC_OnDie()
    {
        OnDie?.Invoke();
    }
}