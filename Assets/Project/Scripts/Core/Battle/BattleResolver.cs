using System.Collections.Generic;

public class BattleResolver
{
    private readonly TargetSelector m_targetSelector;
    private readonly DamageCalculator m_damageCalculator;

    public BattleResolver(TargetSelector _targetSelector, DamageCalculator _damageCalculator)
    {
        m_targetSelector = _targetSelector;
        m_damageCalculator = _damageCalculator;
    }

    public BattleResolution Resolve(IReadOnlyList<BattleArea> _battleAreas)
    {
        BattleResolution resolution = new();
        HashSet<UnitModel> processedAttackers = new();

        CreateAttackEvents(_battleAreas, resolution, processedAttackers);

        ApplyDamage(resolution);
        FindDeadUnits(_battleAreas, resolution);

        return resolution;
    }

    private void CreateAttackEvents(IReadOnlyList<BattleArea> _battleAreas, BattleResolution _resolution,
        HashSet<UnitModel> _processedAttackers)
    {
        foreach (BattleArea battleArea in _battleAreas)
        {
            foreach (UnitModel attacker in battleArea.Participants)
            {
                if (attacker == null || attacker.IsDead)
                    continue;

                if (!_processedAttackers.Add(attacker))
                    continue;

                var target = m_targetSelector.SelectTarget(attacker, battleArea.Participants);

                if (target == null)
                    continue;

                var damage = m_damageCalculator.CalculateDamage(attacker, target);

                var attackEvent = new AttackEvent(attacker, target, damage);

                _resolution.AddAttackEvent(attackEvent);
            }
        }
    }

    private static void ApplyDamage(BattleResolution _resolution)
    {
        foreach (AttackEvent attackEvent in _resolution.AttackEvents)
        {
            attackEvent.Target.TakeDamage(attackEvent.Damage);
        }
    }

    private static void FindDeadUnits(
        IReadOnlyList<BattleArea> _battleAreas,
        BattleResolution _resolution)
    {
        foreach (BattleArea battleArea in _battleAreas)
        {
            foreach (UnitModel participant in battleArea.Participants)
            {
                if (!participant.IsDead)
                    continue;

                _resolution.AddDeadUnit(participant);
            }
        }
    }
}