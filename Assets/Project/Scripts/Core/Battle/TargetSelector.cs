using System.Collections.Generic;

public sealed class TargetSelector
{
    public UnitModel SelectTarget(
        UnitModel _attacker,
        IReadOnlyCollection<UnitModel> _participants)
    {
        if (_attacker == null || _attacker.IsDead)
            return null;

        if (_participants == null)
            return null;

        UnitModel bestTarget = null;

        foreach (UnitModel candidate in _participants)
        {
            if (!IsValidTarget(_attacker, candidate))
                continue;

            if (bestTarget == null ||
                IsBetterTarget(_attacker, candidate, bestTarget))
            {
                bestTarget = candidate;
            }
        }

        return bestTarget;
    }

    private static bool IsValidTarget(
        UnitModel _attacker,
        UnitModel _candidate)
    {
        if (_candidate == null)
            return false;

        if (_candidate.IsDead)
            return false;

        if (_candidate.Owner == _attacker.Owner)
            return false;

        int distance = GridUtility.GetChebyshevDistance(
            _attacker.Position,
            _candidate.Position);

        return distance <= _attacker.AttackRange;
    }

    private static bool IsBetterTarget(
        UnitModel _attacker,
        UnitModel _candidate,
        UnitModel _currentBest)
    {
        int candidatePriority = GetRolePriority(
            _attacker.Role,
            _candidate.Role);

        int currentPriority = GetRolePriority(
            _attacker.Role,
            _currentBest.Role);

        if (candidatePriority != currentPriority)
        {
            return candidatePriority < currentPriority;
        }

        int candidateDistance = GridUtility.GetChebyshevDistance(
            _attacker.Position,
            _candidate.Position);

        int currentDistance = GridUtility.GetChebyshevDistance(
            _attacker.Position,
            _currentBest.Position);

        if (candidateDistance != currentDistance)
        {
            return candidateDistance < currentDistance;
        }

        if (_candidate.CurrentHp != _currentBest.CurrentHp)
        {
            return _candidate.CurrentHp < _currentBest.CurrentHp;
        }

        bool candidateIsCommander =
            _candidate.Role == UnitRole.Commander;

        bool currentIsCommander =
            _currentBest.Role == UnitRole.Commander;

        if (candidateIsCommander != currentIsCommander)
        {
            return candidateIsCommander;
        }

        // 모든 조건이 같으면 ID가 낮은 기물을 선택해
        // 실행 결과를 결정적으로 유지한다.
        return _candidate.Id < _currentBest.Id;
    }

    private static int GetRolePriority(
        UnitRole _attackerRole,
        UnitRole _targetRole)
    {
        switch (_attackerRole)
        {
            case UnitRole.AntiTank:
                return _targetRole == UnitRole.Tank ? 0 : 1;

            case UnitRole.Tank:
                return _targetRole == UnitRole.Infantry ||
                       _targetRole == UnitRole.MachineGunner
                    ? 0
                    : 1;

            case UnitRole.MachineGunner:
                return _targetRole == UnitRole.Infantry ? 0 : 1;

            default:
                return 0;
        }
    }
}