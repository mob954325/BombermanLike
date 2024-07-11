#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_02_Bomb : TestBase
{
    public Transform target;
    public BombBehaviour bomb;

    protected override void OnTest1(InputAction.CallbackContext context)
    {
        target = transform.GetChild(0);

            Runner.Spawn(bomb,
                target.position,
                Quaternion.identity,
                Object.InputAuthority,
                (runner, o) =>
                {
                    o.GetComponent<BombBehaviour>().Init(Util.WorldToGrid(target.position));
                });
    }
}
#endif