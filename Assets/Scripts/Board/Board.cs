using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Board : MonoBehaviour
{
    // 보드한테 필요한건?

    // 1. 각 셀 오브젝트 관리
    // 1.1 셀 생성
    // 1.2 셀 파괴
    [SerializeField] private Dictionary<int, Cell> cells = new Dictionary<int, Cell>();

    /// <summary>
    /// 셀 프리팹
    /// </summary>
    [Tooltip("셀 타입 순으로 프리팹 저장")]
    public NetworkPrefabRef[] cellPrefab;

    /// <summary>
    /// 보드 크기
    /// </summary>
    const int boardSize = 13;

    /// <summary>
    /// 보드 초기화 함수
    /// </summary>
    public void Init(NetworkRunner runner)
    {
        // 0, 0 | boardsize, 0 | 0, boardsize | boardsize, boardsize

        for(int y = 0; y < boardSize; y++)
        {
            for(int x = 0; x < boardSize; x++)
            {
                if(y % 2 == 1 && x % 2 == 0)
                {
                    // 가장자리를 제외한 짝수 번째 셀은 벽이다. ( y는 홀수번째 줄, x는 짝수 번째마다)
                    CreateCell(runner, CellType.Wall, CoordinateConversion.GridToWorld(x, y));
                }
                else
                {
                    // 플레이어 주변 한 칸을 제외하고 파괴 가능한 벽이 랜덤으로 배치된다.
                    CreateCell(runner, CellType.Breakable, CoordinateConversion.GridToWorld(x, y));
                }
            }
        }

        //InitCells();
    }

    private void CreateCell(NetworkRunner runner, CellType type, Vector3 position)
    {
        NetworkObject obj = null;
        string name = $"{type}_{position.x}_{position.z}";

        if (runner.IsServer)
        {
            obj = runner.Spawn(cellPrefab[(int)type], position, Quaternion.identity, null,
            (runner, o) =>
            {
                obj = o;
                o.GetComponent<Cell>().Init(type, name, position + Vector3.up, this.transform);
            });
        }
    }

    /// <summary>
    /// 해당 위치가 존재하는지 확인하는 함수 (grid)
    /// </summary>
    /// <param name="grid">그리드 좌표 값</param>
    /// <returns>존재하면 true 아니면 false</returns>
    private bool IsVaildGrid(Vector2Int grid)
    {
        return grid.x > -1 && grid.x < boardSize && grid.y > -1 && grid.y < boardSize;
    }

    // 좌표 함수 ======================================================================================

    private int WorldToIndex(Vector3 position)
    {
        return GridToIndex(CoordinateConversion.WorldToGrid(position));
    }

    private Vector2Int IndexToGrid(int index)
    {
        return new Vector2Int(index % boardSize, index / boardSize);
    }

    private int GridToIndex(Vector2Int grid)
    {
        return grid.x + grid.y;
    }

    private int GridToIndex(Vector3 position)
    {
        return GridToIndex(CoordinateConversion.WorldToGrid(position));
    }
}