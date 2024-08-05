using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Addons.Physics;

/// <summary>
/// 플레이어 움직임 클래스
/// </summary>
public class PlayerMovement : NetworkBehaviour
{
    private NetworkRigidbody3D rigid;
    private PlayerBehaviour playerBehaviour;

    private NetworkObject body;

    public float speed = 5f;

    /// <summary>
    /// 폭탄 설치 버튼 딜레이 타이머
    /// </summary>
    [Networked]
    private TickTimer setBombDelay { get; set; }

    [Networked]
    private Vector3 inputDir { get; set; }

    /// <summary>
    /// 움직임 속도
    /// </summary>
    [Networked]
    public float moveSpeed { get; set; }

    /// <summary>
    /// 폭탄 설치 딜레이 시간
    /// </summary>
    public float setBombDelayTime = 0.5f;

    private ChangeDetector changeDetector;

    private void Awake()
    {
        rigid = GetComponent<NetworkRigidbody3D>();
        playerBehaviour = GetComponent<PlayerBehaviour>();
        body = rigid.transform.GetChild(0).GetChild(0).GetComponent<NetworkObject>();
    }

    public override void Spawned()
    {
        changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    public override void FixedUpdateNetwork()
    {
        playerBehaviour.SetGridPosition(rigid.transform.position);

        // 움직임 
        if (GetInput(out PlayerInputData data)) // get PlayerInputData
        {
            rigid.transform.Translate(Time.fixedDeltaTime * speed * data.direction);
            moveSpeed = speed * data.direction.sqrMagnitude;
            inputDir = data.direction;
        }

        // 폭탄 설치
        if (data.buttons.IsSet(PlayerButtons.Attack) && setBombDelay.ExpiredOrNotRunning(Runner)) // 공격 버튼 눌렀는지 확인
        {
            setBombDelay = TickTimer.CreateFromSeconds(Runner, setBombDelayTime);
            playerBehaviour.SetBomb(CoordinateConversion.GetGridCenter(playerBehaviour.GetGridPosition(), Board.CellSize));
        }

        foreach(var change in changeDetector.DetectChanges(this))
        {
            if(change == nameof(inputDir))
            {
                body.transform.LookAt(rigid.transform.position + inputDir);
                break;
            }
        }
    }
}