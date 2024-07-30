using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

/// <summary>
/// 플레이어 행동 클래스
/// </summary>
public class PlayerBehaviour : NetworkBehaviour, IHealth
{
    public EffectManager effectManager;

    /// <summary>
    /// 폭탄 프리팹
    /// </summary>
    public BombBehaviour bombPrefab;

    /// <summary>
    /// 플레이어 모델 머터리얼
    /// </summary>
    [SerializeField] private Material bodyMaterial;

    private ChangeDetector changeDetector;

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
    /// 입력 허가 체크 변수
    /// </summary>
    private bool InputsAllowed;

    /// <summary>
    /// 스폰 확인 여부(초기화 후 true)
    /// </summary>
    [Networked]
    private bool isSpawed { get; set; } = false;

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

            if(hp < 0) // 사망 처리
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

    // 유니티 함수 ===============================================================================

    private void Awake()
    {
        Transform child = transform.GetChild(0);
        bodyMaterial = child.GetComponent<MeshRenderer>().material;
    }

    // Fusion 함수 ===============================================================================

    public override void Spawned()
    {
        changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        effectManager.ClearParticles();
    }

    public override void Render()
    {
        foreach(var change in changeDetector.DetectChanges(this))
        {
            switch(change)
            {
                case nameof(isSpawed):
                    // 색상 변경
                    bodyMaterial.color = objColor;
                    break;
            }
        }

        bodyMaterial.color = Color.Lerp(bodyMaterial.color, objColor, Time.deltaTime);
    }

    // 기능 함수 =================================================================================

    /// <summary>
    /// 폭탄 설치하는 함수
    /// </summary>
    public void SetBomb(Vector3 worldGridPosition)
    {
        Runner.Spawn(bombPrefab,
            worldGridPosition,
            Quaternion.identity,
            Object.InputAuthority,
            (runner, o) =>
            {
                o.GetComponent<BombBehaviour>().Init(CurrentGrid, this);
            });
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
        hp = maxHp;
        No = index;

        isSpawed = true;
    }

    /// <summary>
    /// 폭탄 폭발 이팩트 함수 (이팩트 매니저를 위해서 플레이어에 작성)
    /// </summary>
    /// <param name="position">폭발하는 위치</param>
    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    public void RPC_ExplosionEffect(Vector3 position)
    {
        effectManager.PlayParticle(0, position);
    }

    /// <summary>
    /// 그리드 위치를 설정하는 함수
    /// </summary>
    public void SetGridPosition(Vector3 worldPosition)
    {
        CurrentGrid = CoordinateConversion.WorldToGrid(worldPosition);
    }

    public Vector2Int GetGridPosition()
    {
        return CurrentGrid;
    }

    /// <summary>
    /// 공격 면역 함수
    /// </summary>
    private void OnHitImmune()
    {

    }

    /// <summary>
    /// 피격 받을 때 색상 변경 코루틴
    /// </summary>
    /// <param name="prev">이전 색상(플레이어의 원래 색상)</param>
    private IEnumerator hitEffect(Color prev)
    {
        float remainTime = 1f;
        Color curColor = bodyMaterial.color;

        while (remainTime > 0f)
        {
            remainTime -= Time.deltaTime;
            bodyMaterial.color = Color.Lerp(prev, curColor, remainTime);

            yield return null;
        }
    }

    // IHealth =================================================================================

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    public void RPC_OnHit()
    {
        Hp--;
        Debug.Log($"{Hp}");

        Color prevColor = bodyMaterial.color;
        bodyMaterial.color = Color.red;
        hitEffect(prevColor);
    }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    public void RPC_OnDie()
    {
        Debug.Log($"{this.gameObject.name}, {GetComponent<NetworkObject>().Id} : 사망");
    }
}