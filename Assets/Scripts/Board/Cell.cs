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

public class Cell : NetworkBehaviour
{
    CellType type;

    NetworkTRSP trsp;
    NetworkObject networkObject;

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
    }

    public void GetHit()
    {
        networkObject.gameObject.SetActive(false);
    }
}