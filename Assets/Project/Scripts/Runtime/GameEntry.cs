using System.Collections.Generic;
using UnityEngine;

public sealed class GameEntry : MonoBehaviour
{
    [Header("Board")]
    [SerializeField] private BoardView m_boardView;

    [Header("Units")]
    [SerializeField] private UnitView m_unitPrefab;
    [SerializeField] private Transform m_unitRoot;

    [Header("Input")] 
    [SerializeField] private MouseBoardInput m_mouseBoardInput;

    [Header("UI")] 
    [SerializeField] private TurnPanel m_turnPanel;
    [SerializeField] private ActionPanel m_actionPanel;

    private BoardModel m_boardModel;
    private MovementService m_movementService;
    private TurnManager m_turnManager;
    private BattleAreaService m_battleAreaService;

    private readonly List<UnitModel> m_units = new();
    private readonly List<UnitView> m_unitViews = new();

    private UnitActionHistory m_actionHistory;

    private void Start()
    {
        InitializeBoard();
        InitializeUnits();
        InitializeTurn();
        InitializeBattle();
        InitializeInput();
        InitializeUI();

        RefreshAllViews();

        Debug.Log("3D Battle Prototype Started");
    }

    private void InitializeBoard()
    {
        m_boardModel = new BoardModel(10, 10);
        m_movementService = new MovementService(m_boardModel);
        m_boardView.Initialize(m_boardModel);
    }

    private void InitializeUnits()
    {
        int id = 0;

        CreateUnit(new UnitModel(
            id++,
            PlayerSide.Player1,
            UnitRole.Commander,
            new Vector2Int(4, 1),
            _moveRange: 1));

        CreateUnit(new UnitModel(
            id++,
            PlayerSide.Player1,
            UnitRole.Infantry,
            new Vector2Int(3, 3),
            _moveRange: 2));

        CreateUnit(new UnitModel(
            id++,
            PlayerSide.Player1,
            UnitRole.Tank,
            new Vector2Int(5, 1),
            _moveRange: 2));

        CreateUnit(new UnitModel(
            id++,
            PlayerSide.Player2,
            UnitRole.Commander,
            new Vector2Int(4, 8),
            _moveRange: 1));

        CreateUnit(new UnitModel(
            id++,
            PlayerSide.Player2,
            UnitRole.Infantry,
            new Vector2Int(3, 5),
            _moveRange: 2));

        CreateUnit(new UnitModel(
            id++,
            PlayerSide.Player2,
            UnitRole.Tank,
            new Vector2Int(5, 8),
            _moveRange: 2));
    }

    private void CreateUnit(UnitModel _model)
    {
        m_units.Add(_model);
        m_boardModel.PlaceUnit(_model);

        var view = Instantiate(m_unitPrefab, m_unitRoot);
        view.Initialize(_model);

        m_unitViews.Add(view);
    }

    private void InitializeTurn()
    {
        m_turnManager = new TurnManager(m_units);
        m_actionHistory = new UnitActionHistory();

        m_turnManager.StartTurn(PlayerSide.Player1);
    }

    private void InitializeBattle()
    {
        m_battleAreaService = new BattleAreaService(
            m_boardModel,
            m_units,
            _engagementRange: 1,
            _battleRadius: 2);
    }

    private void InitializeInput()
    {
        m_mouseBoardInput.Initialize
        (
            m_boardModel,
            m_boardView,
            m_movementService,
            m_turnManager,
            m_unitViews,
            HandleUnitSelected,
            HandleSelectionCleared,
            HandleUnitActionCompleted,
            HandleMovePreviewChanged,
            HandleMovePreviewCleared
        );
    }

    private void InitializeUI()
    {
        m_turnPanel.Initialize(HandleEndTurnClicked, HandleUndoClicked);

        m_actionPanel.Initialize
        (
            HandleWaitClicked,
            HandleDefendClicked
        );

        m_actionPanel.Hide();
    }

    private void HandleUnitSelected()
    {
        m_actionPanel.Show();
    }

    private void HandleSelectionCleared()
    {
        m_actionPanel.Hide();
    }

    private void HandleWaitClicked()
    {
        m_mouseBoardInput.WaitSelectedUnit();
    }

    private void HandleDefendClicked()
    {
        m_mouseBoardInput.DefendSelectedUnit();
    }

    private void HandleUnitActionCompleted(UnitActionRecord _record)
    {
        m_actionHistory.Push(_record);
        RefreshAllViews();
    }

    private void HandleEndTurnClicked()
    {
        if (!m_turnManager.CanEndTurn())
            return;

        m_boardView.ClearHighlights();
        m_mouseBoardInput.ClearSelection();

        m_actionHistory.Clear();

        m_turnManager.EndTurn();

        RefreshAllViews();

        Debug.Log($"Start Turn : {m_turnManager.CurrentPlayer}");
    }

    private void HandleUndoClicked()
    {
        if (!m_actionHistory.TryPop(out var record))
            return;

        m_mouseBoardInput.ClearSelection();

        var restored = m_boardModel.RestoreUnit(record);

        if (!restored)
        {
            Debug.LogError("Undo failed.");
            RefreshAllViews();
            return;
        }

        var unitView = FindUnitView(record.Unit);

        if (unitView != null)
        {
            unitView.SyncPosition();
            unitView.Refresh();
        }

        RefreshAllViews();

        Debug.Log($"Undo: {record.ActionType} / {record.Unit.Owner} / {record.Unit.Role}");
    }

    private void HandleMovePreviewChanged(UnitModel _unit, Vector2Int _targetPosition)
    {
        List<BattleArea> battleAreas = m_battleAreaService.CreateBattleAreas(_unit, _targetPosition);

        m_boardView.ShowBattlePreviewAreas(battleAreas);

        HashSet<UnitModel> participants = new();

        foreach (BattleArea battleArea in battleAreas)
        {
            foreach (UnitModel participant in battleArea.Participants)
            {
                participants.Add(participant);
            }
        }

        foreach (UnitView unitView in m_unitViews)
        {
            bool isParticipant = participants.Contains(unitView.Model);

            unitView.SetBattlePreviewParticipant(isParticipant);
        }
    }

    private void HandleMovePreviewCleared()
    {
        m_boardView.ClearBattlePreviewAreas();

        foreach (UnitView unitView in m_unitViews)
        {
            unitView.SetBattlePreviewParticipant(false);
        }
    }

    private UnitView FindUnitView(UnitModel _unit)
    {
        foreach (var unitView in m_unitViews)
        {
            if (unitView.Model == _unit)
                return unitView;
        }

        return null;
    }

    private void RefreshAllViews()
    {
        foreach (UnitView unitView in m_unitViews)
        {
            unitView.Refresh();
        }

        RefreshBattleAreas();

        m_turnPanel.Refresh
        (
            _currentPlayer: m_turnManager.CurrentPlayer,
            _turnCount: m_turnManager.TurnCount,
            _canEndTurn: m_turnManager.CanEndTurn(),
            _canUndo: m_actionHistory.CanUndo
        );
    }

    private void RefreshBattleAreas()
    {
        if (m_battleAreaService == null)
            return;

        List<BattleArea> battleAreas = m_battleAreaService.CreateBattleAreas();

        m_boardView.ShowBattleAreas(battleAreas);

        HashSet<UnitModel> participants = new();

        foreach (BattleArea battleArea in battleAreas)
        {
            foreach (UnitModel participant in battleArea.Participants)
            {
                participants.Add(participant);
            }
        }

        foreach (UnitView unitView in m_unitViews)
        {
            bool isParticipant = participants.Contains(unitView.Model);

            unitView.SetBattleParticipant(isParticipant);
        }
    }
}