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

    public float timeToExplosion = 2f;

    private void Awake()
    {
        netObject = GetComponent<NetworkObject>();
    }

    public override void Spawned()
    {
        bombTimer = TickTimer.CreateFromSeconds(Runner, timeToExplosion);
    }

    public override void FixedUpdateNetwork()
    {
        if(!bombTimer.Expired(Runner))
        {
            Despawned(Runner, true);
        }
    }
}