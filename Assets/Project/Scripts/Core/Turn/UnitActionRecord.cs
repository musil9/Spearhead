using UnityEngine;

public class UnitActionRecord
{
    public UnitModel Unit { get; }
    public UnitActionType ActionType { get; }

    public Vector2Int PreviousPosition { get; }
    public bool PreviousHasActed { get; }
    public bool PreviousIsDefending { get; }

    public UnitActionRecord(UnitModel _unit, UnitActionType _actionType)
    {
        Unit = _unit;
        ActionType = _actionType;

        PreviousPosition = _unit.Position;
        PreviousHasActed = _unit.HasActed;
        PreviousIsDefending = _unit.IsDefending;
    }
}
