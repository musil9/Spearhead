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
}
