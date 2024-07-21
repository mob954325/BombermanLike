using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;

/// <summary>
/// 
/// </summary>
public class LevelManager : NetworkSceneManagerDefault
{
    public FusionLauncher Launcher;
    [SerializeField] private LoadingManager loadingManager;

    /// <summary>
    /// 로드 씬 초기화 함수 ( 빈 함수 )
    /// </summary>
    public void ResetLoadedScene() { }

    // Fusion의 로딩 씬 코루틴 ( 씬 전환시 실행되는 코루틴 함수 )
    protected override IEnumerator LoadSceneCoroutine(SceneRef sceneRef, NetworkLoadSceneParameters sceneParams)
    {
        loadingManager.StartLoadingScreen();
        GameManager.instance.SetGameState(GameManager.GameState.Loading);           // 게임 상태 변경 : 로딩
        Launcher.SetConnectionStatus(FusionLauncher.ConnectionStatus.Loading, "");  // 연결 상태 변경 : 로딩
        yield return new WaitForSeconds(1.0f);
        yield return base.LoadSceneCoroutine(sceneRef, sceneParams);                // 로딩?
        Launcher.SetConnectionStatus(FusionLauncher.ConnectionStatus.Loaded, "");   // 연결 상태 변경 : 로드됨
        yield return new WaitForSeconds(1f);
        loadingManager.FinishLoadingScreen();                                       // 로딩 스크린 애니메이션 종료
    }


}