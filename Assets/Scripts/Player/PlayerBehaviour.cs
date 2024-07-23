using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

/// <summary>
/// 플레이어 행동 클래스
/// </summary>
public class PlayerBehaviour : NetworkBehaviour
{
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
    private Vector2Int currentGrid;

    /// <summary>
    /// 플레이어 현재 그리드 위치값 접근 및 수정용 프로퍼티
    /// </summary>
    public Vector2Int CurrnetGrid
    {
        get => currentGrid;
        set
        {
            if(currentGrid != value)
            {
                currentGrid = value;
            }
        }
    }

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
    public void SetBomb()
    {
        Runner.Spawn(bombPrefab,
            transform.position,
            Quaternion.identity,
            Object.InputAuthority,
            (runner, o) =>
            {
                o.GetComponent<BombBehaviour>().Init(CurrnetGrid);
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

    public void InitBeforeSpawn()
    {
        objColor = new Color(Random.value, Random.value, Random.value);
        isSpawed = true;
        Debug.Log("색 선정 완료");
    }
}