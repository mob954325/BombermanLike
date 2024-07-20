using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class GameLauncher : MonoBehaviour
{
    public GameObject LauncherPrefab;

    /// <summary>
    /// 게임 시작시 호출되는 함수
    /// </summary>
    /// <param name="mode">게임모드(ex> host = 4, clinet = 5)</param>
    /// <param name="name">플레이어 이름</param>
    public void Launch(GameMode mode, string name)
    {
        FusionLauncher launcher = FindObjectOfType<FusionLauncher>(); // 퓨전 런처 찾기
        if(launcher == null) // 없으면 생성
        {
            launcher = Instantiate(LauncherPrefab).GetComponent<FusionLauncher>();
        }

        LevelManager levelManager = FindObjectOfType<LevelManager>();
        levelManager.Launcher = launcher;

        launcher.Launch(mode, name, levelManager);
    }
}
