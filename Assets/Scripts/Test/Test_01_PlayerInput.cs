#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Test_01_PlayerInput : TestBase
{
    public int gizmoLength = 5;
    readonly Vector3 GizmoSize = new Vector3(1f, 0f, 1f);

    private void OnDrawGizmos()
    {
        for(int y = 0; y < gizmoLength; y++)
        {
            for(int x = 0; x < gizmoLength; x++)
            {
                Vector3 gridVec = Util.GridToWorld(x, y);
                Handles.DrawWireCube(Util.GetGridCenter(gridVec, GizmoSize.x), GizmoSize);
            }
        }
    }
}
#endif