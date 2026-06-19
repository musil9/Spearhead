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

    public BattleResolution CreateResolution(IReadOnlyList<BattleArea> _battleAreas)
    {
        BattleResolution resolution = new();
        HashSet<UnitModel> processedAttackers = new();

        CreateAttackEvents(_battleAreas, resolution, processedAttackers);

        return resolution;
    }

    private void CreateAttackEvents(IReadOnlyList<BattleArea> _battleAreas, BattleResolution _resolution,
        HashSet<UnitModel> _processedAttackers)
    {
        if (_battleAreas == null) return;

        for (int areaIndex = 0; areaIndex < _battleAreas.Count; areaIndex++)
        {
            BattleArea battleArea = _battleAreas[areaIndex];

            if (battleArea == null) continue;

            foreach (UnitModel attacker in battleArea.Participants)
            {
                if (attacker == null) continue;

                if (attacker.IsDead) continue;

                if (!_processedAttackers.Add(attacker)) continue;

                UnitModel target = m_targetSelector.SelectTarget(attacker, battleArea.Participants);

                if (target == null) continue;

                int damage = m_damageCalculator.CalculateDamage(attacker, target);

                AttackEvent attackEvent = new(attacker, target, damage);

                _resolution.AddAttackEvent(attackEvent);
            }
        }
    }

    public void ApplyResolution(BattleResolution _resolution)
    {
        if (_resolution == null)
            return;

        var attackEvents = _resolution.AttackEvents;

        for (int i = 0; i < attackEvents.Count; i++)
        {
            var attackEvent = attackEvents[i];

            if (attackEvent == null)
                continue;

            if (attackEvent.Target == null)
                continue;

            attackEvent.Target.TakeDamage(attackEvent.Damage);
        }

        CollectDeadUnits(_resolution);
    }

    private static void CollectDeadUnits(BattleResolution _resolution)
    {
        var attackEvents = _resolution.AttackEvents;

        for (int i = 0; i < attackEvents.Count; i++)
        {
            var attackEvent = attackEvents[i];

            if (attackEvent == null)
                continue;

            var target = attackEvent.Target;

            if (target == null)
                continue;

            if (!target.IsDead)
                continue;

            _resolution.AddDeadUnit(target);
        }
    }
}