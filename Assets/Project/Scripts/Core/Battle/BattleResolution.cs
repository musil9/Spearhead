using System.Collections.Generic;

public class BattleResolution
{
    private readonly List<AttackEvent> m_attackEvents = new();
    private readonly HashSet<UnitModel> m_deadUnits = new();

    public IReadOnlyList<AttackEvent> AttackEvents => m_attackEvents;
    public IReadOnlyCollection<UnitModel> DeadUnits => m_deadUnits;

    public void AddAttackEvent(AttackEvent _attackEvent)
    {
        if (_attackEvent == null)
            return;

        m_attackEvents.Add(_attackEvent);
    }

    public void AddDeadUnit(UnitModel _unit)
    {
        if (_unit == null)
            return;

        m_deadUnits.Add(_unit);
    }
}