using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSequencePlayer : MonoBehaviour
{
    [SerializeField] private ProjectileView m_projectilePrefab;
    [SerializeField] private Transform m_effectRoot;

    [Header("Timing")]
    [SerializeField] private float m_projectileDuration = 0.18f;
    [SerializeField] private float m_attackInterval = 0.12f;
    [SerializeField] private float m_hitPause = 0.08f;
    [SerializeField] private float m_deathInterval = 0.1f;

    private IReadOnlyList<UnitView> m_unitViews;

    public void Initialize(IReadOnlyList<UnitView> _unitViews)
    {
        m_unitViews = _unitViews;
    }

    public IEnumerator PlayAttackSequence(BattleResolution _resolution)
    {
        if (_resolution == null)
            yield break;

        var attackEvents = _resolution.AttackEvents;

        for (int i = 0; i < attackEvents.Count; i++)
        {
            var attackEvent = attackEvents[i];

            if (attackEvent == null)
                continue;

            var attackerView = FindUnitView(attackEvent.Attacker);

            var targetView = FindUnitView(attackEvent.Target);

            if (attackerView == null || targetView == null)
            {
                continue;
            }

            attackerView.PlayAttackEffect();

            yield return PlayProjectileRoutine(attackerView.FirePosition, targetView.HitPosition);

            if (m_hitPause > 0f)
            {
                yield return new WaitForSeconds(m_hitPause);
            }

            if (m_attackInterval > 0f)
            {
                yield return new WaitForSeconds(m_attackInterval);
            }
        }
    }

    public IEnumerator PlayDeathSequence(
        BattleResolution _resolution)
    {
        if (_resolution == null)
            yield break;

        List<UnitModel> deadUnits = new();

        foreach (UnitModel deadUnit in _resolution.DeadUnits)
        {
            if (deadUnit == null)
                continue;

            deadUnits.Add(deadUnit);
        }

        deadUnits.Sort(CompareUnitId);

        for (int i = 0; i < deadUnits.Count; i++)
        {
            UnitView unitView =
                FindUnitView(deadUnits[i]);

            if (unitView == null)
                continue;

            yield return unitView.PlayDeathRoutine();

            if (m_deathInterval > 0f)
            {
                yield return new WaitForSeconds(
                    m_deathInterval);
            }
        }
    }

    private IEnumerator PlayProjectileRoutine(
        Vector3 _startPosition,
        Vector3 _targetPosition)
    {
        if (m_projectilePrefab == null)
            yield break;

        Transform parent =
            m_effectRoot != null
                ? m_effectRoot
                : transform;

        ProjectileView projectileView = Instantiate(
            m_projectilePrefab,
            parent);

        yield return projectileView.PlayRoutine(
            _startPosition,
            _targetPosition,
            m_projectileDuration);

        Destroy(projectileView.gameObject);
    }

    private UnitView FindUnitView(UnitModel _unit)
    {
        if (_unit == null || m_unitViews == null)
            return null;

        for (int i = 0; i < m_unitViews.Count; i++)
        {
            UnitView unitView = m_unitViews[i];

            if (unitView == null)
                continue;

            if (unitView.Model == _unit)
                return unitView;
        }

        return null;
    }

    private static int CompareUnitId(
        UnitModel _unitA,
        UnitModel _unitB)
    {
        if (_unitA == null && _unitB == null)
            return 0;

        if (_unitA == null)
            return 1;

        if (_unitB == null)
            return -1;

        return _unitA.Id.CompareTo(_unitB.Id);
    }
}
