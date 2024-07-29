using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

/// <summary>
/// 셀 타임 enum
/// </summary>
public enum CellType
{
    Floor = 0,
    Wall,
    Breakable
}

public class Cell : NetworkBehaviour, IHealth
{
    public CellType type;

    private NetworkTRSP trsp;
    private NetworkObject networkObject;

    private Vector2Int grid = Vector2Int.zero;

    public int Hp { get; set; }
    public int MaxHp { get; set; }

    private void Awake()
    {
        trsp = GetComponent<NetworkTRSP>();
        networkObject = GetComponent<NetworkObject>();
    }

    /// <summary>
    /// Cell 초기화 함수
    /// </summary>
    public void Init(CellType type, string name, Vector3 position, Transform parent = null)
    {
        this.type = type;
        this.gameObject.name = name;

        trsp.transform.parent = parent;
        trsp.transform.localPosition = position;

        grid = CoordinateConversion.WorldToGrid(position);
    }

    public Vector2Int GetGridPosition()
    {
        return grid;
    }
    
    [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
    public void RPC_OnHit()
    {
        networkObject.gameObject.SetActive(false);
    }

    public void RPC_OnDie()
    {
        // 더미
    }
}