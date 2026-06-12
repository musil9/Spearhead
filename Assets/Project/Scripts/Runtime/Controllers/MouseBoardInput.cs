using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class MouseBoardInput : MonoBehaviour
{
    [SerializeField] private Camera m_mainCamera;

    [Header("Layer Masks")] [SerializeField]
    private LayerMask m_tileLayerMask;

    [SerializeField] private LayerMask m_unitLayerMask;

    private BoardModel m_boardModel;
    private BoardView m_boardView;
    private MovementService m_movementService;
    private TurnManager m_turnManager;
    private List<UnitView> m_unitViews = new();

    private TileView m_currentHoverTile;
    private UnitView m_selectedUnitView;

    private readonly List<Vector2Int> m_currentMovablePositions = new();

    private bool m_isInputLocked;
    private Action m_onUnitSelected;
    private Action m_onSelectionCleared;
    private Action m_onUnitActionCompleted;

    public void Initialize(BoardModel _boardModel, BoardView _boardView, MovementService _movementService,
        TurnManager _turnManager, List<UnitView> _unitViews, Action _onUnitSelected, Action _onSelectionCleared,
        Action _onUnitActionCompleted)
    {
        m_boardModel = _boardModel;
        m_boardView = _boardView;
        m_movementService = _movementService;
        m_turnManager = _turnManager;
        m_unitViews = _unitViews;
        m_onUnitSelected = _onUnitSelected;
        m_onSelectionCleared = _onSelectionCleared;
        m_onUnitActionCompleted = _onUnitActionCompleted;
    }

    private void Update()
    {
        if (m_boardModel == null) return;

        if (m_isInputLocked) return;

        UpdateHover();

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleLeftClick();
        }

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ClearSelection();
        }
    }

    public void ClearSelection()
    {
        if (m_selectedUnitView != null)
        {
            m_selectedUnitView.SetSelected(false);
            m_selectedUnitView = null;
        }

        m_currentMovablePositions.Clear();

        if (m_boardView != null)
        {
            m_boardView.ClearHighlights();
        }

        m_onSelectionCleared?.Invoke();
    }

    public void WaitSelectedUnit()
    {
        if (m_selectedUnitView == null)
            return;

        if (!m_turnManager.CanSelect(m_selectedUnitView.Model))
            return;

        var unitView = m_selectedUnitView;

        unitView.Model.Wait();

        CompleteSelectedUnitAction(unitView);
    }

    public void DefendSelectedUnit()
    {
        if (m_selectedUnitView == null)
            return;

        if (!m_turnManager.CanSelect(m_selectedUnitView.Model))
            return;

        var unitView = m_selectedUnitView;

        unitView.Model.Defend();

        CompleteSelectedUnitAction(unitView);
    }

    private void CompleteSelectedUnitAction(UnitView _unitView)
    {
        _unitView.SetSelected(false);
        _unitView.Refresh();

        m_selectedUnitView = null;
        m_currentMovablePositions.Clear();

        if (m_boardView != null)
        {
            m_boardView.ClearHighlights();
        }

        m_onSelectionCleared?.Invoke();
        m_onUnitActionCompleted?.Invoke();
    }

    private void HandleLeftClick()
    {
        if (TryClickUnit()) return;

        TryClickTile();
    }

    private bool TryClickUnit()
    {
        var unit = RaycastUnit();

        if (unit == null) return false;

        if (!m_turnManager.CanSelect(unit.Model))
        {
            Debug.Log($"Cannot select unit: {unit.Model.Owner} / {unit.Model.Role} / HasActed:{unit.Model.HasActed}");
            return true;
        }

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

        m_onUnitSelected?.Invoke();
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

        if (m_selectedUnitView == null) return;

        if (!m_currentMovablePositions.Contains(tile.GridPosition))
        {
            Debug.Log("This tile is not movable.");
            return;
        }

        StartCoroutine(MoveSelectedUnitRoutine(tile.GridPosition));
    }

    private IEnumerator MoveSelectedUnitRoutine(Vector2Int _targetPosition)
    {
        if (m_selectedUnitView == null) yield break;

        m_isInputLocked = true;

        UnitView movingUnitView = m_selectedUnitView;

        bool moved = m_boardModel.MoveUnit(movingUnitView.Model, _targetPosition);

        if (!moved)
        {
            Debug.Log("Move failed.");
            m_isInputLocked = false;
            yield break;
        }

        m_boardView.ClearHighlights();

        yield return movingUnitView.MoveToPosition(_targetPosition);

        movingUnitView.SetSelected(false);
        movingUnitView.Refresh();

        if (m_selectedUnitView == movingUnitView)
        {
            m_selectedUnitView = null;
        }

        m_currentMovablePositions.Clear();

        m_onSelectionCleared?.Invoke();
        m_onUnitActionCompleted?.Invoke();

        m_isInputLocked = false;
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
        if (m_mainCamera == null || Mouse.current == null) return null;

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = m_mainCamera.ScreenPointToRay(mousePosition);

        if (!Physics.Raycast(ray, out var hit, 100f, m_unitLayerMask)) return null;

        return hit.collider.GetComponentInParent<UnitView>();
    }

    private TileView RaycastTile()
    {
        if (m_mainCamera == null || Mouse.current == null) return null;

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = m_mainCamera.ScreenPointToRay(mousePosition);

        if (!Physics.Raycast(ray, out var hit, 100f, m_tileLayerMask)) return null;

        return hit.collider.GetComponentInParent<TileView>();
    }
}