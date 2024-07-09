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
    NetworkObject netObject;

    [Networked]
    private TickTimer bombTimer { get; set; }

    public float timeToExplosion = 3f;

    private void Awake()
    {
        netObject = GetComponent<NetworkObject>();
    }

    /// <summary>
    /// 폭탄 초기화 함수
    /// </summary>
    public void Init()
    {
        //Debug.Log("폭탄 생성");
        gameObject.name = $"Bomb_{Id}";
    }

    public override void Spawned()
    {
        if (Object.HasStateAuthority == false)
            return;

        bombTimer = TickTimer.CreateFromSeconds(Runner, timeToExplosion);
    }

    public override void FixedUpdateNetwork()
    {
        if (bombTimer.Expired(Runner) == false)
            return;
        
        Runner.Despawn(Object);        
    }
}