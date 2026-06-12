using System.Collections;
using UnityEngine;

public class UnitView : MonoBehaviour
{
    [SerializeField] private GameObject m_selectionRing;
    [SerializeField] private GameObject m_actedMark;
    [SerializeField] private GameObject m_defendMark;

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

        ApplyOwnerMaterial();
        SetSelected(false);
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
}
