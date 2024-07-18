using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void Init()
    {

    }    

    public override void Spawned()
    {

    }
}