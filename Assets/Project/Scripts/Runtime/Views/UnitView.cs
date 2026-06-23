using System.Collections;
using UnityEngine;

public class UnitView : MonoBehaviour
{
    [SerializeField] private Transform m_visualRoot;
    [SerializeField] private Transform m_firePoint;
    [SerializeField] private Transform m_hitPoint;
    [SerializeField] private GameObject m_muzzleFlash;
    [SerializeField] private WorldHpBarView m_hpBarView;
    [SerializeField] private Transform m_damageTextPoint;

    [Header("Battle Animation")]
    [SerializeField] private float m_attackPulseDuration = 0.12f;
    [SerializeField] private float m_attackPulseScale = 1.1f;

    [SerializeField] private float m_hitDuration = 0.2f;
    [SerializeField] private float m_hitShakeDistance = 0.08f;
    [SerializeField] private int m_hitShakeCount = 3;

    [SerializeField] private float m_deathDuration = 0.45f;
    [SerializeField] private float m_deathTiltAngle = 75f;

    private Coroutine m_attackEffectCoroutine;
    private Coroutine m_hitEffectCoroutine;

    public Vector3 FirePosition
    {
        get
        {
            if (m_firePoint != null)
                return m_firePoint.position;

            return transform.position + Vector3.up * 0.5f;
        }
    }

    public Vector3 HitPosition
    {
        get
        {
            if (m_hitPoint != null)
                return m_hitPoint.position;

            return transform.position + Vector3.up * 0.4f;
        }
    }

    public Vector3 DamageTextPosition
    {
        get
        {
            if (m_damageTextPoint != null)
                return m_damageTextPoint.position;

            return HitPosition + Vector3.up * 0.25f;
        }
    }

    [SerializeField] private GameObject m_selectionRing;
    [SerializeField] private GameObject m_actedMark;
    [SerializeField] private GameObject m_defendMark;
    [SerializeField] private GameObject m_battleParticipantMark;
    [SerializeField] private GameObject m_battlePreviewParticipantMark;

    [Header("Visual")]
    [SerializeField] private MeshRenderer[] m_renderers;
    [SerializeField] private Material m_player1Material;
    [SerializeField] private Material m_player2Material;
    [SerializeField] private Material m_actedMaterial;

    [Header("Move Animation")] 
    [SerializeField]
    private float m_moveDuration = 0.25f;
    [SerializeField] private AnimationCurve m_moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public UnitModel Model { get; private set; }
    public bool IsMoving { get; private set; }

    public void Initialize(UnitModel _model)
    {
        Model = _model;

        gameObject.name = $"Unit_{_model.Owner}_{_model.Role}_{_model.Id}";
        transform.position = GridUtility.GridToUnitWorld(_model.Position);

        if (m_hpBarView != null)
        {
            m_hpBarView.Initialize(
                _model.CurrentHp,
                _model.MaxHp);
        }

        if (m_muzzleFlash != null)
        {
            m_muzzleFlash.SetActive(false);
        }

        ApplyOwnerMaterial();

        SetSelected(false);
        SetBattleParticipant(false);
        SetBattlePreviewParticipant(false);

        Refresh();
    }

    public void SetSelected(bool _selected)
    {
        if (m_selectionRing != null)
        {
            m_selectionRing.SetActive(_selected);
        }
    }

    public void Refresh()
    {
        if (Model == null)
            return;

        if (Model.IsDead)
        {
            SetSelected(false);
            SetBattleParticipant(false);
            SetBattlePreviewParticipant(false);

            gameObject.SetActive(false);
            return;
        }

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        if (m_actedMark != null)
        {
            m_actedMark.SetActive(Model.HasActed);
        }

        if (m_defendMark != null)
        {
            m_defendMark.SetActive(Model.IsDefending);
        }

        if (Model.HasActed)
        {
            ApplyActedMaterial();
        }
        else
        {
            ApplyOwnerMaterial();
        }
    }

    public void SyncPosition()
    {
        if (Model == null)
            return;

        transform.position = GridUtility.GridToUnitWorld(Model.Position);
    }

    public IEnumerator MoveToPosition(Vector2Int _targetGridPosition)
    {
        if (IsMoving)
            yield break;

        IsMoving = true;

        var startPosition = transform.position;
        var targetPosition = GridUtility.GridToUnitWorld(_targetGridPosition);

        var elapsed = 0f;

        while (elapsed < m_moveDuration)
        {
            elapsed += Time.deltaTime;

            var normalizedTime = Mathf.Clamp01(elapsed / m_moveDuration);
            var curvedTime = m_moveCurve.Evaluate(normalizedTime);

            Vector3 currentPosition = Vector3.Lerp(
                startPosition,
                targetPosition,
                curvedTime);

            float hop = Mathf.Sin(normalizedTime * Mathf.PI) * 0.15f;
            currentPosition.y += hop;

            transform.position = currentPosition;
            yield return null;
        }

        transform.position = targetPosition;
        IsMoving = false;
    }

    private void ApplyOwnerMaterial()
    {
        var material = Model.Owner == PlayerSide.Player1
            ? m_player1Material
            : m_player2Material;

        ApplyMaterial(material);
    }

    private void ApplyActedMaterial()
    {
        if (m_actedMaterial == null)
            return;

        ApplyMaterial(m_actedMaterial);
    }

    private void ApplyMaterial(Material _material)
    {
        if (_material == null || m_renderers == null)
            return;

        foreach (var meshRenderer in m_renderers)
        {
            if (meshRenderer != null)
            {
                meshRenderer.sharedMaterial = _material;
            }
        }
    }

    public void SetBattleParticipant(bool _isParticipant)
    {
        if (m_battleParticipantMark != null)
        {
            m_battleParticipantMark.SetActive(_isParticipant);
        }
    }

    public void SetBattlePreviewParticipant(bool _isParticipant)
    {
        if (m_battlePreviewParticipantMark != null)
        {
            m_battlePreviewParticipantMark.SetActive(_isParticipant);
        }
    }

    private Transform GetVisualTransform()
    {
        if (m_visualRoot != null)
            return m_visualRoot;

        return transform;
    }

    public void PlayAttackEffect()
    {
        if (m_attackEffectCoroutine != null)
        {
            StopCoroutine(m_attackEffectCoroutine);
        }

        m_attackEffectCoroutine = StartCoroutine(PlayAttackEffectRoutine());
    }

    private IEnumerator PlayAttackEffectRoutine()
    {
        var visualTransform = GetVisualTransform();

        var originalScale = visualTransform.localScale;

        var pulseScale = originalScale * m_attackPulseScale;

        if (m_muzzleFlash != null)
        {
            m_muzzleFlash.SetActive(true);
        }

        var halfDuration = Mathf.Max(0.01f, m_attackPulseDuration * 0.5f);

        var elapsedTime = 0f;

        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.deltaTime;

            float normalizedTime = Mathf.Clamp01(elapsedTime / halfDuration);

            visualTransform.localScale = Vector3.Lerp(originalScale, pulseScale, normalizedTime);

            yield return null;
        }

        if (m_muzzleFlash != null)
        {
            m_muzzleFlash.SetActive(false);
        }

        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.deltaTime;

            float normalizedTime =
                Mathf.Clamp01(elapsedTime / halfDuration);

            visualTransform.localScale = Vector3.Lerp(pulseScale, originalScale, normalizedTime);

            yield return null;
        }

        visualTransform.localScale = originalScale;
        m_attackEffectCoroutine = null;
    }

    public void PlayHpChange(
        int _previousHp,
        int _currentHp,
        float _duration)
    {
        if (m_hpBarView == null)
            return;

        if (Model == null)
            return;

        m_hpBarView.PlayChange(
            _previousHp,
            _currentHp,
            Model.MaxHp,
            _duration);
    }

    public void SyncHpImmediate()
    {
        if (m_hpBarView == null)
            return;

        if (Model == null)
            return;

        m_hpBarView.SetImmediate(
            Model.CurrentHp,
            Model.MaxHp);
    }

    public void PlayHitEffect()
    {
        if (m_hitEffectCoroutine != null)
        {
            StopCoroutine(m_hitEffectCoroutine);
        }

        m_hitEffectCoroutine =
            StartCoroutine(PlayHitEffectRoutine());
    }

    private IEnumerator PlayHitEffectRoutine()
    {
        Transform visualTransform = GetVisualTransform();

        Vector3 originalPosition = visualTransform.localPosition;

        int shakeCount = Mathf.Max(1, m_hitShakeCount);

        float singleShakeDuration =
            m_hitDuration / shakeCount;

        for (int i = 0; i < shakeCount; i++)
        {
            float direction =
                i % 2 == 0 ? 1f : -1f;

            Vector3 targetPosition = originalPosition + Vector3.right * direction * m_hitShakeDistance;

            float elapsedTime = 0f;

            while (elapsedTime < singleShakeDuration)
            {
                elapsedTime += Time.deltaTime;

                float normalizedTime =
                    Mathf.Clamp01(
                        elapsedTime / singleShakeDuration);

                float wave =
                    Mathf.Sin(normalizedTime * Mathf.PI);

                visualTransform.localPosition =
                    Vector3.Lerp(
                        originalPosition,
                        targetPosition,
                        wave);

                yield return null;
            }
        }

        visualTransform.localPosition = originalPosition;
        m_hitEffectCoroutine = null;
    }

    public IEnumerator PlayDeathRoutine()
    {
        SetSelected(false);
        SetBattleParticipant(false);
        SetBattlePreviewParticipant(false);

        if (m_muzzleFlash != null)
        {
            m_muzzleFlash.SetActive(false);
        }

        Transform visualTransform = GetVisualTransform();

        Vector3 originalPosition =
            visualTransform.localPosition;

        Vector3 originalScale =
            visualTransform.localScale;

        Quaternion originalRotation =
            visualTransform.localRotation;

        Vector3 targetPosition =
            originalPosition + Vector3.down * 0.25f;

        Vector3 targetScale =
            originalScale * 0.15f;

        Quaternion targetRotation =
            originalRotation *
            Quaternion.Euler(
                m_deathTiltAngle,
                0f,
                m_deathTiltAngle * 0.35f);

        float elapsedTime = 0f;
        float duration = Mathf.Max(0.01f, m_deathDuration);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float normalizedTime =
                Mathf.Clamp01(elapsedTime / duration);

            float easedTime =
                normalizedTime * normalizedTime;

            visualTransform.localPosition = Vector3.Lerp(
                originalPosition,
                targetPosition,
                easedTime);

            visualTransform.localScale = Vector3.Lerp(
                originalScale,
                targetScale,
                easedTime);

            visualTransform.localRotation =
                Quaternion.Slerp(
                    originalRotation,
                    targetRotation,
                    easedTime);

            yield return null;
        }

        if (m_hpBarView != null)
        {
            m_hpBarView.SetVisible(false);
        }

        gameObject.SetActive(false);
    }
}
