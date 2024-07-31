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

    private float time = 0f;

    // Fusion 함수 ======================================================================================

    public override void Spawned()
    {
        netObj = GetComponent<NetworkObject>();
        rigid = GetComponent<NetworkRigidbody3D>();
        body = netObj.transform.GetChild(0).gameObject;
    }

    public override void FixedUpdateNetwork()
    {
        time += Time.fixedDeltaTime;
        body.transform.Translate(Vector3.up * Mathf.Sin(time * 2f) * Time.fixedDeltaTime);
    }

    public void OnPick()
    {
        Destroy(netObj.gameObject);
        //netObj.gameObject.SetActive(false);
    }
}