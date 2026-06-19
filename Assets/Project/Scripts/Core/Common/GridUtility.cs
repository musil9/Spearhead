using UnityEngine;

public static class GridUtility
{
    public static Vector3 GridToWorld(Vector2Int _gridPosition)
    {
        return new Vector3(_gridPosition.x, 0f, _gridPosition.y);
    }

    public static Vector3 GridToUnitWorld(Vector2Int _gridPosition)
    {
        return new Vector3(_gridPosition.x, 0.15f, _gridPosition.y);
    }

    public static Vector2Int WorldToGrid(Vector3 _worldPosition)
    {
        return new Vector2Int(Mathf.RoundToInt(_worldPosition.x), Mathf.RoundToInt(_worldPosition.z));
    }

    /// <summary>
    /// 상하좌우 이동 거리.
    /// 대각선 한 칸은 거리 2로 계산된다.
    /// </summary>
    public static int GetManhattanDistance(
        Vector2Int _positionA,
        Vector2Int _positionB)
    {
        int deltaX = Mathf.Abs(_positionA.x - _positionB.x);
        int deltaY = Mathf.Abs(_positionA.y - _positionB.y);

        return deltaX + deltaY;
    }

    /// <summary>
    /// 상하좌우와 대각선 인접 거리.
    /// 대각선 한 칸도 거리 1로 계산된다.
    /// </summary>
    public static int GetChebyshevDistance(
        Vector2Int _positionA,
        Vector2Int _positionB)
    {
        int deltaX = Mathf.Abs(_positionA.x - _positionB.x);
        int deltaY = Mathf.Abs(_positionA.y - _positionB.y);

        return Mathf.Max(deltaX, deltaY);
    }
}
