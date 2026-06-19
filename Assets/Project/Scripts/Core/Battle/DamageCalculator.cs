using UnityEngine;

public class DamageCalculator
{
    private const int DefendBonus = 1;

    public int CalculateDamage(UnitModel _attacker, UnitModel _defender)
    {
        if (_attacker == null || _defender == null)
            return 0;

        int attack = _attacker.AttackPower;
        int defense = _defender.Defense;
        int bonusDamage = GetRoleBonusDamage(_attacker, _defender);

        if (_defender.IsDefending)
        {
            defense += DefendBonus;
        }

        int damage = attack + bonusDamage - defense;

        return Mathf.Max(1, damage);
    }

    private int GetRoleBonusDamage(UnitModel _attacker, UnitModel _defender)
    {
        if (_attacker.Role == UnitRole.AntiTank &&
            _defender.Role == UnitRole.Tank)
        {
            return 3;
        }

        if (_attacker.Role == UnitRole.Tank &&
            _defender.Role is
                UnitRole.Infantry or
                UnitRole.MachineGunner)
        {
            return 1;
        }

        if (_attacker.Role == UnitRole.MachineGunner &&
            _defender.Role == UnitRole.Infantry)
        {
            return 1;
        }

        return 0;
    }
}
