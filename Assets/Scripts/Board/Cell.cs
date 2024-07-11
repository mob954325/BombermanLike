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

    /// <summary>
    /// Cell 초기화 함수
    /// </summary>
    public void Init(CellType type, string name, Transform parent = null)
    {
        this.type = type;
        this.gameObject.name = name;
        transform.parent = parent;
    }
}