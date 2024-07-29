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
    NetworkRigidbody3D rigid;
    PlayerBehaviour playerBehaviour;

    public float speed = 5f;

    /// <summary>
    /// 폭탄 설치 버튼 딜레이 타이머
    /// </summary>
    [Networked]
    private TickTimer setBombDelay { get; set; }

    /// <summary>
    /// 움직임 속도
    /// </summary>
    public float moveSpeed = 5f;

    /// <summary>
    /// 폭탄 설치 딜레이 시간
    /// </summary>
    public float setBombDelayTime = 0.5f;

    private void Awake()
    {
        rigid = GetComponent<NetworkRigidbody3D>();
        playerBehaviour = GetComponent<PlayerBehaviour>();
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
            return;

        playerBehaviour.SetGridPosition(rigid.transform.position);

        // 움직임 
        if (GetInput(out PlayerInputData data)) // get PlayerInputData
        {
            rigid.transform.Translate(Time.fixedDeltaTime * speed * data.direction);
        }

        // 폭탄 설치
        if (data.buttons.IsSet(PlayerButtons.Attack) && setBombDelay.ExpiredOrNotRunning(Runner)) // 공격 버튼 눌렀는지 확인
        {
            setBombDelay = TickTimer.CreateFromSeconds(Runner, setBombDelayTime);
            playerBehaviour.SetBomb(CoordinateConversion.GetGridCenter(playerBehaviour.GetGridPosition(), Board.CellSize));
        }

        // 퍼즈 매뉴
        if (data.buttons.IsSet(PlayerButtons.Pause))
        {
            GameManager.instance.ShowExitScreen();
        }
        else 
        {
            GameManager.instance.CloseExitScreen();
        }
    }
}