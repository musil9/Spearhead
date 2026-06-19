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
    private Action<UnitActionRecord> m_onUnitActionCompleted;
    private Action<UnitModel, Vector2Int> m_onMovePreviewChanged;
    private Action m_onMovePreviewCleared;

    public void Initialize(
        BoardModel _boardModel,
        BoardView _boardView,
        MovementService _movementService,
        TurnManager _turnManager,
        List<UnitView> _unitViews,
        Action _onUnitSelected,
        Action _onSelectionCleared,
        Action<UnitActionRecord> _onUnitActionCompleted,
        Action<UnitModel, Vector2Int> _onMovePreviewChanged,
        Action _onMovePreviewCleared)
    {
        m_boardModel = _boardModel;
        m_boardView = _boardView;
        m_movementService = _movementService;
        m_turnManager = _turnManager;
        m_unitViews = _unitViews;

        m_onUnitSelected = _onUnitSelected;
        m_onSelectionCleared = _onSelectionCleared;
        m_onUnitActionCompleted = _onUnitActionCompleted;

        m_onMovePreviewChanged = _onMovePreviewChanged;
        m_onMovePreviewCleared = _onMovePreviewCleared;
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

        m_onMovePreviewCleared?.Invoke();
        m_onSelectionCleared?.Invoke();
    }

    public void WaitSelectedUnit()
    {
        if (m_selectedUnitView == null)
            return;

        m_onMovePreviewCleared?.Invoke();

        if (!m_turnManager.CanSelect(m_selectedUnitView.Model))
            return;

        var unitView = m_selectedUnitView;

        var record = new UnitActionRecord(unitView.Model, UnitActionType.Wait);

        unitView.Model.Wait();

        CompleteSelectedUnitAction(unitView, record);
    }

    public void DefendSelectedUnit()
    {
        if (m_selectedUnitView == null)
            return;

        m_onMovePreviewCleared?.Invoke();

        if (!m_turnManager.CanSelect(m_selectedUnitView.Model))
            return;

        var unitView = m_selectedUnitView;

        var record = new UnitActionRecord(unitView.Model, UnitActionType.Defend);

        unitView.Model.Defend();

        CompleteSelectedUnitAction(unitView, record);
    }

    private void CompleteSelectedUnitAction(UnitView _unitView, UnitActionRecord _record)
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
        m_onUnitActionCompleted?.Invoke(_record);
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

        List<Vector2Int> movablePositions = m_movementService.GetMovablePositions(_unitView.Model);

        m_currentMovablePositions.AddRange(movablePositions);

        m_boardView.ShowMovableTiles(m_currentMovablePositions);

        m_onUnitSelected?.Invoke();

        m_onMovePreviewCleared?.Invoke();
        RefreshCurrentMovePreview();
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
        if (m_selectedUnitView == null)
            yield break;

        m_isInputLocked = true;
        m_onMovePreviewCleared?.Invoke();

        UnitView movingUnitView = m_selectedUnitView;

        var record = new UnitActionRecord(movingUnitView.Model, UnitActionType.Move);

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
        m_onUnitActionCompleted?.Invoke(record);

        m_isInputLocked = false;
    }

    private void UpdateHover()
    {
        TileView tile = RaycastTile();

        if (m_currentHoverTile == tile)
            return;

        if (m_currentHoverTile != null)
        {
            m_boardView.RestoreHoverVisual(m_currentHoverTile);
        }

        m_onMovePreviewCleared?.Invoke();

        m_currentHoverTile = tile;

        if (m_currentHoverTile == null)
            return;

        m_currentHoverTile.SetHover();

        RefreshCurrentMovePreview();
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

    private void RefreshCurrentMovePreview()
    {
        if (m_selectedUnitView == null)
            return;

        if (m_currentHoverTile == null)
            return;

        Vector2Int targetPosition = m_currentHoverTile.GridPosition;

        if (!m_currentMovablePositions.Contains(targetPosition))
            return;

        m_onMovePreviewChanged?.Invoke(m_selectedUnitView.Model, targetPosition);
    }
}