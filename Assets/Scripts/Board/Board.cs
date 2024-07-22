using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Board : NetworkBehaviour
{
    public NetworkPrefabRef floor;
    public NetworkPrefabRef wall;
    public NetworkPrefabRef[] block;

    /// <summary>
    /// 보드 크기
    /// </summary>
    private const int boardSize = 14;

    /// <summary>
    /// 테두리 위치 배열 (시작 위치, 끝 위치 )
    /// </summary>
    private readonly int[] outLinePositions = { -1, boardSize };

    /// <summary>
    /// 생성 확인 여부
    /// </summary>
    private bool isGenerated = false;

    /// <summary>
    /// 보드 생성 함수
    /// </summary>
    /// <param name="runner">로컬 러너</param>
    public void GenerateBoard(NetworkRunner runner)
    {
        if (isGenerated) return;

        GameObject Floors = new GameObject("Floor Container");
        GameObject Walls = new GameObject("Walls Container");
        GameObject Blocks = new GameObject("Blocks Container");
        Floors.transform.parent = transform;
        Walls.transform.parent = transform;
        Blocks.transform.parent = transform;

        // 바닥 생성
        for (int y = 0; y < boardSize; y++)
        {
            for (int x = 0; x < boardSize; x++)
            {
                CreateCell(runner, CellType.Floor, floor, new Vector3(x, 0, y), $"Floor {x}_{y}", Floors.transform);            // 바닥 생성
                CreateCell(runner, CellType.Breakable, block[0], new Vector3(x, 1, y), $"Block {x}_{y}", Blocks.transform);     // 장애물 생성
            }
        }

        int k = 0; // 코너 생성용 지역변수

        // 벽 생성
        for (int i = 0; i < outLinePositions.Length * 2; i++)
        {
            // 4방향 코너 생성
            CreateCell(runner, CellType.Wall, wall,
                        new Vector3(outLinePositions[i % 2], 0, outLinePositions[(i + k) % 2]),
                        $"Wall_Cornor_{i}",
                        Walls.transform);

            for (int j = 0; j < boardSize; j++)
            {
                if (i < 2)
                {
                    // 왼쪽 오른쪽
                    CreateCell(runner, CellType.Wall, block[0],
                                new Vector3(outLinePositions[i % 2], 0, j),
                                $"Wall_Vertical_{outLinePositions[i % 2]}_{j}",
                                Walls.transform);
                }
                else
                {
                    // 위 아래
                    CreateCell(runner, CellType.Wall, block[0],
                                new Vector3(j, 0, outLinePositions[i % 2]),
                                $"Wall_Horizontal_{outLinePositions[i % 2]}_{j}",
                                Walls.transform);
                }
            }

            if (i == 0 || i == 2) k++;
        }

        isGenerated = true;
    }

    /// <summary>
    /// 맵의 Cell을 생성하는 함수
    /// </summary>
    /// <param name="runner">로컬 러너</param>
    /// <param name="type">셀 타입</param>
    /// <param name="cellPrefab">프리팹 오브젝트</param>
    /// <param name="position">위치</param>
    /// <param name="name">오브젝트 이름 (없으면 정해진 이름 설정)</param>
    /// <param name="parent">오브젝트 부모 (없으면 null)</param>
    private void CreateCell(NetworkRunner runner, CellType type, NetworkPrefabRef cellPrefab, Vector3 position, string name = "Cell_Unknown", Transform parent = null)
    {
        runner.Spawn
            (cellPrefab, 
            position,
            Quaternion.identity, 
            Object.InputAuthority, // error
            (runner, o) => 
            {
                o.GetComponent<Cell>().Init(type, name, parent); 
            });
    }
}
