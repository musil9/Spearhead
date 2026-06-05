using UnityEngine;

public sealed class MouseBoardInput : MonoBehaviour
{
    [SerializeField] private Camera m_mainCamera;
    [SerializeField] private LayerMask m_tileLayerMask;

    private TileView m_currentHoverTile;

    private void Update()
    {
        UpdateHover();

        if (Input.GetMouseButtonDown(0))
        {
            TryClickTile();
        }
    }

    private void UpdateHover()
    {
        var tile = RaycastTile();

        if (m_currentHoverTile == tile)
            return;

        if (m_currentHoverTile != null)
        {
            m_currentHoverTile.SetDefault();
        }

        m_currentHoverTile = tile;

        if (m_currentHoverTile != null)
        {
            m_currentHoverTile.SetHover();
        }
    }

    private void TryClickTile()
    {
        var tile = RaycastTile();

        if (tile == null)
            return;

        Debug.Log($"Clicked Tile: {tile.GridPosition}");
    }

    private TileView RaycastTile()
    {
        Ray ray = m_mainCamera.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out var hit, 100f, m_tileLayerMask))
            return null;

        return hit.collider.GetComponentInParent<TileView>();
    }
}