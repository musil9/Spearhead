public class AttackEvent
{
    public UnitModel Attacker { get; }
    public UnitModel Target { get; }
    public int Damage { get; }

    public AttackEvent(UnitModel _attacker, UnitModel _target, int _damage)
    {
        Attacker = _attacker;
        Target = _target;
        Damage = _damage;
    }
}