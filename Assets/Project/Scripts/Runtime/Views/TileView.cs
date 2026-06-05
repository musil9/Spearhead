using System;
using UnityEngine;

public class TileView : MonoBehaviour
{
    [SerializeField] private MeshRenderer m_meshRenderer;

    [SerializeField] private Material m_materialA;
    [SerializeField] private Material m_materialB;
    [SerializeField] private Material m_hoverMaterial;

    private Material m_defaultMaterial;

    public Vector2Int GridPosition { get; private set; }

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
        m_meshRenderer.sharedMaterial = m_defaultMaterial;
    }

    public void SetHover()
    {
        m_meshRenderer.sharedMaterial = m_hoverMaterial;
    }
}
