using System.Collections.Generic;
using UnityEngine;

public class MovementService
{
    private readonly BoardModel m_boardModel;

    public MovementService(BoardModel _board)
    {
        m_boardModel = _board;
    }

    public List<Vector2Int> GetMovablePositions(UnitModel _unit)
    {
        var result = new List<Vector2Int>();

        for (int y = -_unit.MoveRange; y <= _unit.MoveRange; y++)
        {
            for (int x = -_unit.MoveRange; x <= _unit.MoveRange; x++)
            {
                var distance = Mathf.Abs(x) + Mathf.Abs(y);

                if (distance == 0)
                    continue;

                if (distance > _unit.MoveRange)
                    continue;

                var position = _unit.Position + new Vector2Int(x, y);

                if (!m_boardModel.CanMoveTo(position))
                    continue;

                result.Add(position);
            }
        }

        return result;
    }
}