using UnityEngine;
using UnityEngine.InputSystem;

public sealed class MouseBoardInput : MonoBehaviour
{
    [SerializeField] private Camera m_mainCamera;
    [SerializeField] private LayerMask m_tileLayerMask;
    [SerializeField] private LayerMask m_unitLayerMask;

    private TileView m_currentHoverTile;
    private UnitView m_selectedUnit;

   private void Update()
    {
        UpdateHover();

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleLeftClick();
        }
    }

    private void HandleLeftClick()
    {
        if (TryClickUnit())
            return;

        TryClickTile();
    }

    private bool TryClickUnit()
    {
        var unit = RaycastUnit();

        if (unit == null)
            return false;

        SelectUnit(unit);

        Debug.Log($"Clicked Unit: {unit.Model.Owner} / {unit.Model.Role} / Id:{unit.Model.Id}");
        return true;
    }

    private void SelectUnit(UnitView unit)
    {
        if (m_selectedUnit != null)
        {
            m_selectedUnit.SetSelected(false);
        }

        m_selectedUnit = unit;
        m_selectedUnit.SetSelected(true);
    }

    private void TryClickTile()
    {
        var tile = RaycastTile();

        if (tile == null)
        {
            Debug.Log("No tile clicked");
            return;
        }

        Debug.Log($"Clicked Tile: {tile.GridPosition}");
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

    private UnitView RaycastUnit()
    {
        if (m_mainCamera == null || Mouse.current == null)
            return null;

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = m_mainCamera.ScreenPointToRay(mousePosition);

        if (!Physics.Raycast(ray, out var hit, 100f, m_unitLayerMask))
            return null;

        return hit.collider.GetComponentInParent<UnitView>();
    }

    private TileView RaycastTile()
    {
        if (m_mainCamera == null || Mouse.current == null)
            return null;

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = m_mainCamera.ScreenPointToRay(mousePosition);

        if (!Physics.Raycast(ray, out var hit, 100f, m_tileLayerMask))
            return null;

        return hit.collider.GetComponentInParent<TileView>();
    }
}