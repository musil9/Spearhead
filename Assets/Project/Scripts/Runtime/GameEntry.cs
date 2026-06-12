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

    [Header("UI")] [SerializeField] private TurnPanel m_turnPanel;

    private BoardModel m_boardModel;
    private MovementService m_movementService;
    private TurnManager m_turnManager;

    private readonly List<UnitModel> m_units = new();
    private readonly List<UnitView> m_unitViews = new();

    private void Start()
    {
        InitializeBoard();
        InitializeUnits();
        InitializeTurn();
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
            new Vector2Int(3, 1),
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
            new Vector2Int(3, 8),
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
        m_turnManager.StartTurn(PlayerSide.Player1);
    }

    private void InitializeInput()
    {
        m_mouseBoardInput.Initialize(m_boardModel, m_boardView, m_movementService, m_turnManager, m_unitViews, HandleUnitActionCompleted);
    }

    private void InitializeUI()
    {
        m_turnPanel.Initialize(HandleEndTurnClicked);
    }

    private void HandleUnitActionCompleted()
    {
        RefreshAllViews();
    }

    private void HandleEndTurnClicked()
    {
        if (!m_turnManager.CanEndTurn())
            return;

        m_boardView.ClearHighlights();
        m_mouseBoardInput.ClearSelection();

        m_turnManager.EndTurn();

        RefreshAllViews();

        Debug.Log($"Start Turn : {m_turnManager.CurrentPlayer}");
    }

    private void RefreshAllViews()
    {
        foreach (UnitView unitView in m_unitViews)
        {
            unitView.Refresh();
        }

        m_turnPanel.Refresh(m_turnManager.CurrentPlayer, m_turnManager.TurnCount, m_turnManager.CanEndTurn());
    }
}