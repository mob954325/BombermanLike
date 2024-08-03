using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Addons.Physics;

public class ItemObject : NetworkBehaviour
{
    private NetworkObject netObj;
    private NetworkRigidbody3D rigid;

    private GameObject body;

    [Networked]
    private float time { get; set; }

    private void Awake()
    {
        netObj = GetComponent<NetworkObject>();
        rigid = GetComponent<NetworkRigidbody3D>();        
    }

    private void FixedUpdate()
    {
        time += Time.fixedDeltaTime;
        body.transform.Translate(Vector3.up * Mathf.Sin(time * 2f) * Time.fixedDeltaTime);
    }

    // Fusion 함수 ======================================================================================

    public override void Spawned()
    {
        body = netObj.transform.GetChild(0).gameObject;
    }

    public override void FixedUpdateNetwork()
    {
    }

    public void OnPick()
    {
        Destroy(netObj.gameObject);
    }
}