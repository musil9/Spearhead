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

    public bool HasActed { get; private set; }
    public bool IsDefending { get; private set; }
    public bool IsDead { get; private set; }

    public UnitModel(int _id, PlayerSide _owner, UnitRole _role, Vector2Int _position, int _moveRange)
    {
        Id = _id;
        Owner = _owner;
        Role = _role;
        Position = _position;
        MoveRange = _moveRange;
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

    public void RestoreActionState(Vector2Int _position, bool _hasActed, bool _isDefending)
    {
        Position = _position;
        HasActed = _hasActed;
        IsDefending = _isDefending;
    }

    public void Kill()
    {
        IsDead = true;
    }
}
