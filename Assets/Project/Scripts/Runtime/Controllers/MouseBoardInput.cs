using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class MouseBoardInput : MonoBehaviour
{
    [SerializeField] private Camera m_mainCamera;

    [Header("Layer Masks")]
    [SerializeField] private LayerMask m_tileLayerMask;
    [SerializeField] private LayerMask m_unitLayerMask;

    private BoardModel m_boardModel;
    private BoardView m_boardView;
    private MovementService m_movementService;

    private TileView m_currentHoverTile;
    private UnitView m_selectedUnitView;
    private readonly List<Vector2Int> m_currentMovablePositions = new();

    public void Initialize(BoardModel _boardModel, BoardView _boardView, MovementService _movementService)
    {
        m_boardModel = _boardModel;
        m_boardView = _boardView;
        m_movementService = _movementService;
    }

   private void Update()
   {
       if (m_boardModel == null)
           return;

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

    private void SelectUnit(UnitView _unitView)
    {
        if (m_selectedUnitView != null)
        {
            m_selectedUnitView.SetSelected(false);
        }

        m_selectedUnitView = _unitView;
        m_selectedUnitView.SetSelected(true);

        m_currentMovablePositions.Clear();

        var movablePositions = m_movementService.GetMovablePositions(_unitView.Model);
        m_currentMovablePositions.AddRange(movablePositions);

        m_boardView.ShowMovableTiles(m_currentMovablePositions);
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

        if (m_selectedUnitView == null)
            return;

        if (!m_currentMovablePositions.Contains(tile.GridPosition))
        {
            Debug.Log("This tile is not movable.");
            return;
        }

        bool moved = m_boardModel.MoveUnit(m_selectedUnitView.Model, tile.GridPosition);

        if (!moved)
        {
            Debug.Log("Move failed.");
            return;
        }

        m_selectedUnitView.SyncPosition();
        m_selectedUnitView.SetSelected(false);
        m_selectedUnitView = null;

        m_currentMovablePositions.Clear();
        m_boardView.ClearHighlights();
    }

    private void UpdateHover()
    {
        var tile = RaycastTile();

        if (m_currentHoverTile == tile)
            return;

        if (m_currentHoverTile != null)
        {
            m_boardView.RestoreHoverVisual(m_currentHoverTile);
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