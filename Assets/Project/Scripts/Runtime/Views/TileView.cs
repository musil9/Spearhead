using System;
using UnityEngine;

public class TileView : MonoBehaviour
{
    [SerializeField] private MeshRenderer m_meshRenderer;

    [SerializeField] private Material m_materialA;
    [SerializeField] private Material m_materialB;
    [SerializeField] private Material m_hoverMaterial;
    [SerializeField] private Material m_movableMaterial;

    private Material m_defaultMaterial;
    private bool m_isMovable;

    public Vector2Int GridPosition { get; private set; }
    public bool IsMovable => m_isMovable;

    public void Initialize(Vector2Int _gridPosition)
    {
        GridPosition = _gridPosition;

        gameObject.name = $"Tile ({_gridPosition.x}, {_gridPosition.y})";
        transform.position = GridUtility.GridToWorld(_gridPosition);

        bool isEven = (_gridPosition.x + _gridPosition.y) % 2 == 0;
        m_defaultMaterial = isEven ? m_materialA : m_materialB;

        SetDefault();
    }

    public void SetDefault()
    {
        m_isMovable = false;
        m_meshRenderer.sharedMaterial = m_defaultMaterial;
    }

    public void SetHover()
    {
        if (m_isMovable)
            return;

        m_meshRenderer.sharedMaterial = m_hoverMaterial;
    }

    public void SetMovable()
    {
        m_isMovable = true;
        m_meshRenderer.sharedMaterial = m_movableMaterial;
    }

    public void RestoreVisual()
    {
        if (m_isMovable)
        {
            m_meshRenderer.sharedMaterial = m_movableMaterial;
            return;
        }

        m_meshRenderer.sharedMaterial = m_defaultMaterial;
    }
}
