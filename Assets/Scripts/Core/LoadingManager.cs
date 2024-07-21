using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager Instance;

    [SerializeField] private Animator loadingScreenAnimator;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        //DontDestroyOnLoad(gameObject);
    }

    public void LoadPlayLevel(NetworkRunner runner)
    {
        runner.LoadScene("GamePlay");
    }

    /// <summary>
    /// 로딩 애니메이션 시작 함수
    /// </summary>
    public void StartLoadingScreen()
    {
        loadingScreenAnimator.Play("In");
    }

    /// <summary>
    /// 로딩 애니메이션 끝 
    /// </summary>
    public void FinishLoadingScreen()
    {
        loadingScreenAnimator.Play("Out");
    }
}