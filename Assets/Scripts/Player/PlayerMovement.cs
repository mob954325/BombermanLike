using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

/// <summary>
/// 플레이어 움직임 클래스
/// </summary>
public class PlayerMovement : NetworkBehaviour
{
    public float moveSpeed = 5f;

    NetworkCharacterController networkCharacterController;

    private void Awake()
    {
        networkCharacterController = GetComponent<NetworkCharacterController>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out PlayerInputData data)) // get PlayerInputData
        {
            networkCharacterController.Move(Runner.DeltaTime * moveSpeed * data.direction);
        }
    }
}