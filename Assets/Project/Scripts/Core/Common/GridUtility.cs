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

    public static int GetDistance(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }
}
