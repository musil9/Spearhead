using System.Collections.Generic;
using UnityEngine;

public class VictoryChecker
{
    private readonly IReadOnlyList<UnitModel> m_units;

    public VictoryChecker(IReadOnlyList<UnitModel> _units)
    {
        m_units = _units;
    }

    public GameResult GetResult()
    {
        var player1Defeated = IsDefeated(PlayerSide.Player1);
        var player2Defeated = IsDefeated(PlayerSide.Player2);

        if (player1Defeated && player2Defeated)
            return GameResult.Draw;

        if (player1Defeated)
            return GameResult.Player2Win;

        if (player2Defeated)
            return GameResult.Player1Win;

        return GameResult.None;
    }

    private bool IsDefeated(PlayerSide _player)
    {
        bool hasCommander = false;
        bool commanderAlive = false;
        bool anyUnitAlive = false;

        for (int i = 0; i < m_units.Count; i++)
        {
            UnitModel unit = m_units[i];

            if (unit.Owner != _player)
                continue;

            if (!unit.IsDead)
            {
                anyUnitAlive = true;
            }

            if (unit.Role != UnitRole.Commander)
                continue;

            hasCommander = true;

            if (!unit.IsDead)
            {
                commanderAlive = true;
            }
        }

        if (!hasCommander)
            return true;

        if (!commanderAlive)
            return true;

        if (!anyUnitAlive)
            return true;

        return false;
    }
}