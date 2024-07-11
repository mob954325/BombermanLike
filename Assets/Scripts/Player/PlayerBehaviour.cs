using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

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

    [Networked] 
    public int playerId { get; set; }

    [Networked]
    public Color playerColor { get; set; }

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
}