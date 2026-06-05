using System.Collections.Generic;
using UnityEngine;

public class BoardModel
{
    private readonly Dictionary<Vector2Int, TileModel> m_tiles = new();

    public int Width { get; }
    public int Height { get; }

    public BoardModel(int _width, int _height)
    {
        Width = _width;
        Height = _height;

        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                var position = new Vector2Int(x, y);
                m_tiles[position] = new TileModel(position);
            }
        }
    }

    public bool IsInside(Vector2Int _position)
    {
        return _position.x >= 0 && _position.y >= 0 && _position.x < Width && _position.y < Height;
    }

    public TileModel GetTile(Vector2Int _position)
    {
        return m_tiles.TryGetValue(_position, out var tile)
            ? tile
            : null;
    }

    public IEnumerable<TileModel> GetAllTiles()
    {
        return m_tiles.Values;
    }

    public bool CanMoveTo(Vector2Int _position)
    {
        if (!IsInside(_position))
            return false;

        var tile = GetTile(_position);

        return tile != null && !tile.IsOccupied;
    }

    public void PlaceUnit(UnitModel _unit)
    {
        var tile = GetTile(_unit.Position);

        if (tile == null)
            return;

        if (tile.IsOccupied)
            return;

        tile.SetUnit(_unit);
    }

    public bool MoveUnit(UnitModel _unit, Vector2Int _targetPosition)
    {
        if (!CanMoveTo(_targetPosition))
            return false;

        var currentTile = GetTile(_unit.Position);
        var targetTile = GetTile(_targetPosition);

        if (currentTile == null || targetTile == null)
            return false;

        currentTile.ClearUnit();
        targetTile.SetUnit(_unit);

        _unit.MoveTo(_targetPosition);

        return true;
    }
}
