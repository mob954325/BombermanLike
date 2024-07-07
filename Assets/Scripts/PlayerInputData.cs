using UnityEngine;
using Fusion;

/// <summary>
/// 플레이어 인풋 데이터
/// </summary>
public struct PlayerInputData : INetworkInput
{
    public Vector3 direction;
}
