using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Fusion;

namespace FusionUtilsEvents
{
    /// <summary>
    /// Fusion 전용 이벤트 스크립터블 오브젝트 클래스
    /// </summary>
    [CreateAssetMenu]
    public class FusionEvent : ScriptableObject
    {
        /// <summary>
        /// 해당 이벤트가 응답할 델리게이트 리스트
        /// </summary>
        public List<Action<PlayerRef, NetworkRunner>> Responses = new List<Action<PlayerRef, NetworkRunner>>();

        /// <summary>
        /// 리스트의 모든 응답 함수들 실행 함수
        /// </summary>
        /// <param name="player">실행할 클라이언트 플레이어 (없으면 defualt)</param>
        /// <param name="runner">네트워크 러너 (없으면 null)</param>
        public void ExecuteResponses(PlayerRef player = default, NetworkRunner runner = null)
        {
            foreach(var action in Responses)    // foreach문으로 Responses 내용 실행
            {
                action.Invoke(player, runner);
            }
        }

        /// <summary>
        /// 이벤트가 실행할 델리게이트를 리스트에 추가하는 함수
        /// </summary>
        /// <param name="response">추가할 델리게이트</param>
        public void RegisterResponse(Action<PlayerRef, NetworkRunner> response)
        {
            Responses.Add(response);
        }

        /// <summary>
        /// 이벤트 리스트에서 델리게이트를 제거하는 함수
        /// </summary>
        /// <param name="response">제거할 델리게이트</param>
        public void RemoveResponse(Action<PlayerRef, NetworkRunner> response)
        {
            if(Responses.Contains(response))    // 리스트에 해당 델리게이트가 있는지 확인
            {
                Responses.Remove(response);
            }
        }
    }
}