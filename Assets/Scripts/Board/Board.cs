using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Unity.VisualScripting;
using Fusion.Addons.Physics;

public class Board : MonoBehaviour
{
    /// <summary>
    /// 셀 딕셔너리
    /// </summary>
    private Dictionary<int, Cell> cells = new Dictionary<int, Cell>();

    /// <summary>
    /// 셀 프리팹
    /// </summary>
    [Tooltip("셀 타입 순으로 프리팹 저장")]
    public NetworkPrefabRef[] cellPrefab;

    /// <summary>
    /// 보드 크기
    /// </summary>
    const int BoardSize = 13;

    /// <summary>
    /// 셀 하나의 사이즈 (중앙값 계산 용)
    /// </summary>
    public static float CellSize = 1f;

    /// <summary>
    /// 보드 초기화 함수
    /// </summary>
    public void Init(NetworkRunner runner, out List<Cell> cells)
    {
        List<Cell> cellList = new List<Cell>();

        for(int y = 0; y < BoardSize; y++)
        {
            for(int x = 0; x < BoardSize; x++)
            {
                if (IsNearSpawn(x, y))
                    continue;

                if(y % 2 == 1 && x % 2 == 1)    // 가장자리를 제외한 짝수 번째 셀은 벽이다. ( y는 홀수번째 줄, x는 짝수 번째마다)
                {                    
                    CreateCell(runner, CellType.Wall, CoordinateConversion.GetGridCenter(x, y, CellSize), out _);
                }
                else // 플레이어 주변 한 칸을 제외하고 파괴 가능한 벽이 랜덤으로 배치된다.
                {                    
                    float rand = Random.value;
                    if(rand > 0.3)
                    {
                        CreateCell(runner, CellType.Breakable, CoordinateConversion.GetGridCenter(x, y, CellSize), out Cell cell);
                        cellList.Add(cell);
                    }
                }
            }
        }

        cells = cellList;
    }

    private void CreateCell(NetworkRunner runner, CellType type, Vector3 position, out Cell createdCell)
    {
        //NetworkObject obj = null;
        Cell cell = null;
        string name = $"{type}_{position.x}_{position.z}";

        if (runner.IsServer)
        {
            if (!runner.TryGetComponent(out RunnerSimulatePhysics3D comp))
            {
                runner.AddComponent<RunnerSimulatePhysics3D>();
            }

            runner.Spawn(cellPrefab[(int)type], position, Quaternion.identity, null,
            (runner, o) =>
            {
                cell = o.GetComponent<Cell>();
                cell.Init(type, name, position + Vector3.up, GetComponent<NetworkTRSP>().transform);
            });
        }

        createdCell = cell;
    }

    /// <summary>
    /// 스폰 지점에 가까운지 확인하는 함수 (근처 1칸씩, 대각선 제외)
    /// </summary>
    /// <param name="x">x 값</param>
    /// <param name="y">y 값</param>
    /// <returns>스폰 지점이면 true 아니면 false</returns>
    private bool IsNearSpawn(int x, int y)
    {
        bool result = false;

        if((x < 2 && y < 2)
            || (x < 2 && y > BoardSize - 3)
            || (x > BoardSize - 3 && y > BoardSize - 3)
            || (x > BoardSize - 3 && y < 2))
        {
            result = true;
        }


        return result;
    }

    /// <summary>
    /// 해당 위치가 존재하는지 확인하는 함수 (grid)
    /// </summary>
    /// <param name="grid">그리드 좌표 값</param>
    /// <returns>존재하면 true 아니면 false</returns>
    private bool IsVaildGrid(Vector2Int grid)
    {
        return grid.x > -1 && grid.x < BoardSize && grid.y > -1 && grid.y < BoardSize;
    }

    // 좌표 함수 ======================================================================================

    private int WorldToIndex(Vector3 position)
    {
        return GridToIndex(CoordinateConversion.WorldToGrid(position));
    }

    private Vector2Int IndexToGrid(int index)
    {
        return new Vector2Int(index % BoardSize, index / BoardSize);
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