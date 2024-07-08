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

    public Vector2Int currentGrid;
    public float moveSpeed = 5f;

    private void Awake()
    {
        characterController = GetComponent<NetworkCharacterController>();
    }

    public override void Spawned()
    {
        currentGrid = Util.WorldToGrid(transform.position);
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out PlayerInputData data)) // get PlayerInputData
        {
            characterController.Move(moveSpeed * Runner.DeltaTime * data.direction);
        }

        currentGrid = Util.WorldToGrid(transform.position);
    }
}