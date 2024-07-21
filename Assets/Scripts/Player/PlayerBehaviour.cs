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
    /// 플레이어 고유 Id
    /// </summary>
    [Networked] 
    public int id { get; set; }

    /// <summary>
    /// 오브젝트 색깔
    /// </summary>
    [Networked]
    public Color objColor { get; set; }

    /// <summary>
    /// 입력 허가 체크 변수
    /// </summary>
    [Networked]
    private bool InputsAllowed { get; set; }

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

    public void Init(int playerId, Color color)
    {
        id = playerId;
        objColor = color;
    }
}