#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_03_Map : TestBase
{
    Board board;

    private void Start()
    {
        board = FindAnyObjectByType<Board>();
    }

    protected override void OnTest1(InputAction.CallbackContext context)
    {
    }
}
#endif