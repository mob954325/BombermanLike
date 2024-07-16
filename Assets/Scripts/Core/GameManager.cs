using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;

    public Board board;

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