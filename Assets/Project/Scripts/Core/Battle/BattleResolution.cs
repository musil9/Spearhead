using System.Collections.Generic;

public class BattleResolution
{
    private readonly List<AttackEvent> m_attackEvents = new();
    private readonly List<UnitDamageResult> m_damageResults = new();
    private readonly HashSet<UnitModel> m_deadUnits = new();

    public IReadOnlyList<AttackEvent> AttackEvents => m_attackEvents;
    public IReadOnlyList<UnitDamageResult> DamageResults => m_damageResults;
    public IReadOnlyCollection<UnitModel> DeadUnits => m_deadUnits;

    public bool IsApplied { get; private set; }

    public void AddAttackEvent(AttackEvent _attackEvent)
    {
        if (_attackEvent == null)
            return;

        m_attackEvents.Add(_attackEvent);
    }

    public void AddDamageResult(UnitDamageResult _damageResult)
    {
        if (_damageResult == null)
            return;

        m_damageResults.Add(_damageResult);
    }

    public void AddDeadUnit(UnitModel _unit)
    {
        if (_unit == null)
            return;

        m_deadUnits.Add(_unit);
    }

    public bool TryMarkApplied()
    {
        if (IsApplied)
            return false;

        IsApplied = true;
        return true;
    }
}