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

    NetworkTransform networkTransform;
    NetworkObject networkObject;

    private void Awake()
    {
        networkTransform = GetComponent<NetworkTransform>();
        networkObject = GetComponent<NetworkObject>();
    }

    /// <summary>
    /// Cell 초기화 함수
    /// </summary>
    public void Init(CellType type, string name, Vector3 position, Transform parent = null)
    {
        this.type = type;
        this.gameObject.name = name;

        networkTransform.transform.parent = parent;
        networkTransform.transform.localPosition = position;
    }

    public void GetHit()
    {
        networkObject.gameObject.SetActive(false);
    }
}