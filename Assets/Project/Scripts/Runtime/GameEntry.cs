using System.Collections.Generic;
using UnityEngine;

public sealed class GameEntry : MonoBehaviour
{
    [Header("Board")]
    [SerializeField] private BoardView m_boardView;

    [Header("Units")]
    [SerializeField] private UnitView m_unitPrefab;
    [SerializeField] private Transform m_unitRoot;

    private BoardModel m_boardModel;
    private readonly List<UnitModel> m_units = new();
    private readonly List<UnitView> m_unitViews = new();

    private void Start()
    {
        InitializeBoard();
        InitializeUnits();

        Debug.Log("3D Battle Prototype Started");
    }

    private void InitializeBoard()
    {
        m_boardModel = new BoardModel(10, 10);
        m_boardView.Initialize(m_boardModel);
    }

    private void InitializeUnits()
    {
        var commander = new UnitModel(
            _id: 0,
            _owner: PlayerSide.Player1,
            _role: UnitRole.Commander,
            _position: new Vector2Int(4, 1));

        m_units.Add(commander);

        CreateUnitView(commander);
    }

    private void CreateUnitView(UnitModel model)
    {
        var view = Instantiate(m_unitPrefab, m_unitRoot);
        view.Initialize(model);

        m_unitViews.Add(view);
    }
}