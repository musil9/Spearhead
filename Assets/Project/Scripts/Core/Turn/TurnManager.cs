using System.Collections.Generic;

public class TurnManager
{
    private readonly List<UnitModel> m_units;

    public PlayerSide CurrentPlayer { get; private set; } = PlayerSide.Player1;
    public int TurnCount { get; private set; } = 1;

    public TurnManager(List<UnitModel> _units)
    {
        m_units = _units;
    }

    public void StartTurn(PlayerSide _player)
    {
        CurrentPlayer = _player;

        foreach (var unit in m_units)
        {
            if (unit.Owner == _player && !unit.IsDead)
            {
                unit.ResetTurnState();
            }
        }
    }

    public bool CanSelect(UnitModel _unit)
    {
        if (_unit == null)
            return false;

        if (_unit.IsDead)
            return false;

        if (_unit.Owner != CurrentPlayer)
            return false;

        if (_unit.HasActed)
            return false;

        return true;
    }

    public bool CanEndTurn()
    {
        foreach (var unit in m_units)
        {
            if (unit.Owner != CurrentPlayer)
                continue;

            if (unit.IsDead)
                continue;

            if (!unit.HasActed)
                return false;
        }

        return true;
    }

    public void EndTurn()
    {
        CurrentPlayer = CurrentPlayer == PlayerSide.Player1
            ? PlayerSide.Player2
            : PlayerSide.Player1;

        TurnCount++;

        StartTurn(CurrentPlayer);
    }
}
