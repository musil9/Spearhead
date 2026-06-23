using UnityEngine;

public class UnitDamageResult
{
    public UnitModel Unit { get; }
    public int PreviousHp { get; }
    public int CurrentHp { get; }

    public int TotalDamage => PreviousHp - CurrentHp;

    public UnitDamageResult(UnitModel _unit, int _previousHp, int _currentHp)
    {
        Unit = _unit;
        PreviousHp = _previousHp;
        CurrentHp = _currentHp;
    }
}