
using System.Collections.Generic;
using UnityEngine;

public class BattleAreaService
{
    private readonly BoardModel m_boardModel;
    private readonly IReadOnlyList<UnitModel> m_units;

    private readonly int m_engagementRange;
    private readonly int m_battleRadius;

    public BattleAreaService(BoardModel _boardModel,
        IReadOnlyList<UnitModel> _units,
        int _engagementRange = 1,
        int _battleRadius = 2)
    {
        m_boardModel = _boardModel;
        m_units = _units;
        m_engagementRange = _engagementRange;
        m_battleRadius = _battleRadius;
    }

    public List<BattleArea> CreateBattleAreas(UnitModel _previewUnit = null, Vector2Int? _previewPosition = null)
    {
        var battleAreas = CreateAreasFromEngagements(_previewUnit, _previewPosition);

        MergeOverlappingAreas(battleAreas);
        PopulateParticipants(
            battleAreas,
            _previewUnit,
            _previewPosition);

        return battleAreas;
    }

    private List<BattleArea> CreateAreasFromEngagements(UnitModel _previewUnit, Vector2Int? _previewPosition)
    {
        List<BattleArea> battleAreas = new();

        for (int i = 0; i < m_units.Count; i++)
        {
            var unitA = m_units[i];

            if (unitA.IsDead)
                continue;

            for (int j = 0; j < m_units.Count; j++)
            {
                var unitB = m_units[j];

                if (unitB.IsDead)
                    continue;

                if (unitA.Owner == unitB.Owner)
                    continue;

                Vector2Int positionA = GetPosition(
                    unitA,
                    _previewUnit,
                    _previewPosition);

                Vector2Int positionB = GetPosition(
                    unitB,
                    _previewUnit,
                    _previewPosition);

                int distance = GridUtility.GetChebyshevDistance(positionA, positionB);

                if (distance > m_engagementRange)
                    continue;
   
                BattleArea battleArea = CreateArea(
                    positionA,
                    positionB);

                battleAreas.Add(battleArea);
            }
        }

        return battleAreas;
    }

    private static Vector2Int GetPosition(
        UnitModel _unit,
        UnitModel _previewUnit,
        Vector2Int? _previewPosition)
    {
        if (_unit == _previewUnit && _previewPosition.HasValue)
        {
            return _previewPosition.Value;
        }

        return _unit.Position;
    }

    private BattleArea CreateArea(Vector2Int _positionA, Vector2Int _positionB)
    {
        var battleArea = new BattleArea();

        var center = new Vector2((_positionA.x + _positionB.x) * 0.5f, (_positionA.y + _positionB.y) * 0.5f);

        for (int y = 0; y < m_boardModel.Height; y++)
        {
            for (int x = 0; x < m_boardModel.Width; x++)
            {
                Vector2Int tilePosition = new(x, y);

                float distance =
                    Mathf.Abs(tilePosition.x - center.x) +
                    Mathf.Abs(tilePosition.y - center.y);

                if (distance <= m_battleRadius + 0.001f)
                {
                    battleArea.AddTile(tilePosition);
                }
            }
        }

        return battleArea;
    }

    private void MergeOverlappingAreas(List<BattleArea> _battleAreas)
    {
        bool merged;

        do
        {
            merged = false;

            for (int i = 0; i < _battleAreas.Count; i++)
            {
                for (int j = i + 1; j < _battleAreas.Count; j++)
                {
                    BattleArea areaA = _battleAreas[i];
                    BattleArea areaB = _battleAreas[j];

                    if (!areaA.Overlaps(areaB))
                        continue;

                    areaA.MergeWith(areaB);
                    _battleAreas.RemoveAt(j);

                    merged = true;
                    break;
                }

                if (merged)
                    break;
            }
        }
        while (merged);
    }

    private void PopulateParticipants(
        List<BattleArea> _battleAreas,
        UnitModel _previewUnit,
        Vector2Int? _previewPosition)
    {
        foreach (BattleArea battleArea in _battleAreas)
        {
            foreach (UnitModel unit in m_units)
            {
                if (unit.IsDead)
                    continue;

                Vector2Int position = GetPosition(unit, _previewUnit, _previewPosition);

                if (!battleArea.ContainsTile(position))
                    continue;

                battleArea.AddParticipant(unit);
            }
        }
    }
}
