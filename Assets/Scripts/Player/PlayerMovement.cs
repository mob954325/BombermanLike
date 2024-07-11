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
    NetworkCharacterController characterController;
    PlayerBehaviour playerBehaviour;

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
        characterController = GetComponent<NetworkCharacterController>();
        playerBehaviour = GetComponent<PlayerBehaviour>();
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
            return;

        if (GetInput(out PlayerInputData data)) // get PlayerInputData
        {
            characterController.Move(moveSpeed * Runner.DeltaTime * data.direction);
        }

        if(data.buttons.IsSet(PlayerButtons.Attack) && setBombDelay.ExpiredOrNotRunning(Runner)) // 공격 버튼 눌렀는지 확인
        {
            setBombDelay = TickTimer.CreateFromSeconds(Runner, setBombDelayTime);
            playerBehaviour.SetBomb();
            Debug.Log("PlayerMovemnet : 폭탄 설치");
        }
    }
}