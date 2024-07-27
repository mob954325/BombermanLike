using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 캐릭터 체력 인터페이스
/// </summary>
public interface IHealth
{
    /// <summary>
    /// 캐릭터 현재 체력
    /// </summary>
    public int Hp { get; set; }

    /// <summary>
    /// 캐릭터 최대 체력
    /// </summary>
    public int MaxHp { get; set; }

    /// <summary>
    /// 피격시 실행되는 함수
    /// </summary>
    public void RPC_OnHit();

    /// <summary>
    /// 사망시 실행되는 함수
    /// </summary>
    public void RPC_OnDie();
}