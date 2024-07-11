#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_03_Map : TestBase
{
    Board board;

    public int gizmoLength = 5;
    readonly Vector3 GizmoSize = new Vector3(1f, 0f, 1f);

    private void Start()
    {
        board = FindAnyObjectByType<Board>();
    }

    private void OnDrawGizmos()
    {
        for (int y = 0; y < gizmoLength; y++)
        {
            for (int x = 0; x < gizmoLength; x++)
            {
                Vector3 gridVec = Util.GridToWorld(x, y);
                Handles.color = Color.red;
                Handles.DrawWireCube(Util.GetGridCenter(gridVec, GizmoSize.x), GizmoSize);
            }
        }
    }

    protected override void OnTest1(InputAction.CallbackContext context)
    {
        board.GenerateBoard();
    }
}
#endif