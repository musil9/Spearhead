using System.Collections.Generic;

public class UnitActionHistory
{
    private readonly Stack<UnitActionRecord> m_records = new();

    public bool CanUndo => m_records.Count > 0;

    public void Push(UnitActionRecord _record)
    {
        if (_record == null)
            return;

        m_records.Push(_record);
    }

    public bool TryPop(out UnitActionRecord _record)
    {
        if (m_records.Count <= 0)
        {
            _record = null;
            return false;
        }

        _record = m_records.Pop();
        return true;
    }

    public void Clear()
    {
        m_records.Clear();
    }
}