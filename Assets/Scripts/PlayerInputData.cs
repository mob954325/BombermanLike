using UnityEngine;
using Fusion;

/// <summary>
/// 플레이어 입력 버튼 모음
/// </summary>
public enum PlayerButtons
{
    Attack = 0,
    Pause = 1
}

/// <summary>
/// 플레이어 인풋 데이터
/// </summary>
public struct PlayerInputData : INetworkInput
{
    public NetworkButtons buttons;
    public Vector3 direction;
}
