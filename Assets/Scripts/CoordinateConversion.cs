using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoordinateConversion : MonoBehaviour
{
    /// <summary>
    /// 그리드에서 월즈 좌표값 구하는 함수 (x,y)
    /// </summary>
    /// <param name="x">x 좌표</param>
    /// <param name="y">y 좌표</param>
    /// <returns>월드 좌표 값</returns>
    public static Vector3 GridToWorld(int x, int y)
    {
        return new Vector3(x, 0f, y);
    }

    /// <summary>
    /// 그리드 좌표값에서 월드 좌표 구하는 함수 (그리드)
    /// </summary>
    /// <param name="grid">그리즈 좌표값</param>
    /// <returns>월드 좌표 값</returns>
    public static Vector3 GridToWorld(Vector2Int grid)
    {
        return GridToWorld(grid.x, grid.y);
    }

    /// <summary>
    /// 월드좌표에서 그리드좌표 구하는 함수
    /// </summary>
    /// <param name="worldPosition">월드 좌표값</param>
    /// <returns>그리드 좌표</returns>
    public static Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        return new Vector2Int(Mathf.FloorToInt(worldPosition.x), Mathf.FloorToInt(worldPosition.z));
    }

    /// <summary>
    /// 한 칸의 중앙값을 반환하는 함수
    /// </summary>
    /// <param name="world">월드 좌표 값</param>
    /// <param name="cellLength">한 칸의 길이</param>
    /// <returns>한 칸의 중앙 값</returns>
    public static Vector3 GetGridCenter(Vector3 world, float cellLength)
    {
        return new Vector3(world.x + cellLength * 0.5f, 0f, world.z + cellLength * 0.5f);
    }

    /// <summary>
    /// 한 칸의 중앙값을 반환하는 함수
    /// </summary>
    /// <param name="grid">그리드 값</param>
    /// <param name="cellLength">한 칸의 길이</param>
    /// <returns>한 칸의 중앙 값</returns>
    public static Vector3 GetGridCenter(Vector2Int grid, float cellLength)
    {
        return new Vector3(grid.x + cellLength * 0.5f, 0f, grid.y + cellLength * 0.5f);
    }

    /// <summary>
    /// 한 칸의 중앙값을 반환하는 함수
    /// </summary>
    /// <param name="x">x 좌표</param>
    /// <param name="y">y 좌표</param>
    /// <param name="cellLength">한 칸의 길이</param>
    /// <returns>한 칸의 중앙 값</returns>
    public static Vector3 GetGridCenter(int x, int y, float cellLength)
    {
        return new Vector3(x + cellLength * 0.5f, 0f, y + cellLength * 0.5f);
    }
}