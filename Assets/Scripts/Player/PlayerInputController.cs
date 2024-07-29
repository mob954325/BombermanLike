using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using UnityEngine.InputSystem;

public class PlayerInputController : NetworkBehaviour, INetworkRunnerCallbacks
{
    PlayerInputAction playerInputAction;

    /// <summary>
    /// 플레이어 방향 값
    /// </summary>
    [SerializeField]private Vector3 playerInputDir = Vector3.zero;

    /// <summary>
    /// check isPressed space Button (SetBomb)
    /// </summary>
    private bool isPressedSpace = false;

    /// <summary>
    /// esc버튼을 눌렀는지 확인하는 변수
    /// </summary>
    private bool isPressedEscape = false;

    private void Awake()
    {
        playerInputAction = new PlayerInputAction();
    }

    private void OnDisable()
    {
        OnPlayerEnable();
    }

    private void OnPlayerDisable()
    {
        playerInputAction.Player.Enable();
        playerInputAction.UI.Enable();
        playerInputAction.Player.Move.performed += OnMoveInput;
        playerInputAction.Player.Move.canceled += OnMoveInput;
        playerInputAction.Player.Attack.performed += OnAttackInput;
        playerInputAction.Player.Attack.canceled += OnAttackInput;
        playerInputAction.UI.Exit.performed += OnEscapeInput;
        playerInputAction.UI.Exit.canceled += OnEscapeInput;
    }

    private void OnPlayerEnable()
    {
        playerInputAction.UI.Exit.canceled -= OnEscapeInput;
        playerInputAction.UI.Exit.performed -= OnEscapeInput;
        playerInputAction.Player.Attack.canceled -= OnAttackInput;
        playerInputAction.Player.Attack.performed -= OnAttackInput;
        playerInputAction.Player.Move.canceled -= OnMoveInput;
        playerInputAction.Player.Move.performed -= OnMoveInput;
        playerInputAction.UI.Disable();
        playerInputAction.Player.Disable();
    }

    // 인풋 =========================================================================================

    private void OnMoveInput(InputAction.CallbackContext context)
    {
        Vector2 inputVec = context.ReadValue<Vector2>();
        playerInputDir = new Vector3(inputVec.x, 0, inputVec.y);

        if(playerInputDir.sqrMagnitude > 1) // 1이상, 대각선 방향
        {
            playerInputDir = new Vector3(inputVec.x, 0, 0); // 좌우만 움직이기
        }
    }

    private void OnAttackInput(InputAction.CallbackContext context) // Space
    {
        isPressedSpace = context.performed;
    }

    private void OnEscapeInput(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            isPressedEscape = !isPressedEscape;
        }

        // 화면 활성화 되었을 때 플레이어 움직임 막기
        if(GameManager.instance.IsExitScreenOpen())
        {
            playerInputAction.Player.Disable();
        }
        else
        {
            playerInputAction.Player.Enable();
        }
    }

    // Fusion =========================================================================================

    public override void Spawned() // 스폰 되었을 때 실행
    {
        if(Object.HasInputAuthority)
        {
            Runner.AddCallbacks(this);
            OnPlayerDisable();
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input) // 인풋 받는 함수
    {
        PlayerInputData inputData = new PlayerInputData();

        inputData.direction = playerInputDir;
        inputData.buttons.Set(PlayerButtons.Attack, isPressedSpace);
        inputData.buttons.Set(PlayerButtons.Pause, isPressedEscape);

        input.Set(inputData);
    }

    #region Not used CallBacks
    public void OnConnectedToServer(NetworkRunner runner) { }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }

    public void OnSceneLoadDone(NetworkRunner runner) { }

    public void OnSceneLoadStart(NetworkRunner runner) { }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    #endregion
}