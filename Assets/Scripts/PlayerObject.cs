using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Fusion;

public class PlayerObject : NetworkBehaviour
{
    public float moveSpeed = 5f;

    NetworkCharacterController netCC;

    private void Awake()
    {
        netCC = GetComponent<NetworkCharacterController>();
    }

    public override void FixedUpdateNetwork()
    {
        if(GetInput(out NetworkInputData data))
        {
            netCC.Move(Runner.DeltaTime * moveSpeed * data.direction);
        }
    }
}
