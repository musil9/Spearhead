using UnityEngine;

public enum PlayerSide
{
    Player1,
    Player2,
}

public enum UnitRole
{
    Commander,
    Infantry,
    MachineGunner,
    Tank,
    AntiTank,
}

public class UnitModel
{
    public int Id { get; }
    public PlayerSide Owner { get; }
    public UnitRole Role { get; }

    public Vector2Int Position { get; private set; }

    public int MoveRange { get; }
    public int AttackRange { get; }

    public int MaxHp { get; }
    public int CurrentHp { get; private set; }

    public int AttackPower { get; }
    public int Defense { get; }

    public bool HasActed { get; private set; }
    public bool IsDefending { get; private set; }
    public bool IsDead => CurrentHp <= 0;

    public UnitModel(int _id, PlayerSide _owner, UnitRole _role, Vector2Int _position, int _moveRange
    , int _attackRange, int _maxHp, int _attackPower, int _defense)
    {
        Id = _id;
        Owner = _owner;
        Role = _role;
        Position = _position;

        MoveRange = _moveRange;
        AttackRange = _attackRange;

        MaxHp = _maxHp;
        CurrentHp = _maxHp;

        AttackPower = _attackPower;
        Defense = _defense;
    }

    public void MoveTo(Vector2Int _position)
    {
        Position = _position;
        IsDefending = false;
        HasActed = true;
    }

    public void Wait()
    {
        IsDefending = false;
        HasActed = true;
    }

    public void Defend()
    {
        IsDefending = true;
        HasActed = true;
    }

    public void ResetTurnState()
    {
        HasActed = false;
        IsDefending = false;
    }

    public void TakeDamage(int _damage)
    {
        if (_damage <= 0 || IsDead)
            return;

        CurrentHp = Mathf.Max(0, CurrentHp - _damage);
    }

    public void RestoreActionState(Vector2Int _position, bool _hasActed, bool _isDefending)
    {
        Position = _position;
        HasActed = _hasActed;
        IsDefending = _isDefending;
    }

    public void Kill()
    {
        CurrentHp = 0;
    }
}
