using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using FusionUtilsEvents;
using WebSocketSharp;

/// <summary>
/// 플레이어 데이터 클래스 (NetworkBehaviour)
/// </summary>
public class PlayerData : NetworkBehaviour
{
    /// <summary>
    /// 플레이어 이름
    /// </summary>
    [Networked]
    public NetworkString<_16> nickName { get; set; }

    /// <summary>
    /// 플레이어 오브젝트 인스턴스
    /// </summary>
    [Networked]
    public NetworkObject Instance { get; set; }

    /// <summary>
    /// 플레이어 스폰 이벤트 (FusionEvent)
    /// </summary>
    public FusionEvent OnPlayerDataSpawnedEvent;

    // Networked 변수들 변경 감지용
    private ChangeDetector playerChangeDetector;

    /// <summary>
    /// 플레이어 이름을 설정하는 함수 (플레이어가 호스트에게 요청)
    /// </summary>
    /// <param name="nick">설정할 닉네임</param>
    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void RPC_SetNick(string nick)
    {
        nickName = nick;
    }

    public override void Spawned()
    {
        playerChangeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState, false); // 네트워크 변수 변경 감지 시작
        if(Object.HasInputAuthority)
        {
            string nickName = PlayerPrefs.GetString("Nick", string.Empty);  // 오너? 면 PlayerPrefs에서 값 가져오기
            RPC_SetNick(string.IsNullOrEmpty(nickName) ? $"Player {Object.InputAuthority.AsIndex}" : nickName); // 없으면 기본이름, 있으면 설정한 이름
        }

        DontDestroyOnLoad(this);
        Runner.SetPlayerObject(Object.InputAuthority, Object);  // 러너에 플레이어 오브젝트 설정
        OnPlayerDataSpawnedEvent?.ExecuteResponses(Object.InputAuthority, Runner);  // 이벤트 실행

        if(Object.HasInputAuthority)
        {
            // 게임매니저에 플레이어 데이터 등록 함수 추가 (Fusion 이벤트 추가 함수)
        }
    }

    public override void Render()
    {
        foreach(var change in playerChangeDetector.DetectChanges(this)) // 현재 클래스에서 변화가 발생하면
        {
            switch(change)
            {
                case nameof(nickName):
                    OnPlayerDataSpawnedEvent?.ExecuteResponses(Object.InputAuthority, Runner);  // 이벤트 실행
                    break;
            }
        }
    }
}