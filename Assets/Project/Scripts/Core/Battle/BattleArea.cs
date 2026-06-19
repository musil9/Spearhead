using System.Collections.Generic;
using UnityEngine;

public class BattleArea
{
    private readonly HashSet<Vector2Int> m_tiles = new();
    private readonly HashSet<UnitModel> m_participants = new();

    public IReadOnlyCollection<Vector2Int> Tiles => m_tiles;
    public IReadOnlyCollection<UnitModel> Participants => m_participants;

    public void AddTile(Vector2Int _position)
    {
        m_tiles.Add(_position);
    }

    public bool ContainsTile(Vector2Int _position)
    {
        return m_tiles.Contains(_position);
    }

    public void AddParticipant(UnitModel _unit)
    {
        if (_unit == null)
            return;

        m_participants.Add(_unit);
    }

    public bool Overlaps(BattleArea _other)
    {
        if (_other == null)
            return false;

        return m_tiles.Overlaps(_other.m_tiles);
    }

    public void MergeWith(BattleArea _other)
    {
        if (_other == null)
            return;

        m_tiles.UnionWith(_other.m_tiles);
        m_participants.UnionWith(_other.m_participants);
    }
}
