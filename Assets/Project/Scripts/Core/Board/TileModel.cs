using UnityEngine;

public class TileModel
{
    public Vector2Int Position { get; }
    public UnitModel OccupiedUnit { get; private set; }

    public bool IsOccupied => OccupiedUnit != null;

    public TileModel(Vector2Int _position)
    {
        Position = _position;
    }

    public void SetUnit(UnitModel _unit)
    {
        OccupiedUnit = _unit;
    }

    public void ClearUnit()
    {
        OccupiedUnit = null;
    }
}