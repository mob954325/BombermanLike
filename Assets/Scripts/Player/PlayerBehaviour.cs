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
                o.GetComponent<BombBehaviour>().Init();
            });
    }
}