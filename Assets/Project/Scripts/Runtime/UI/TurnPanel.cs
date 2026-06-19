using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text m_turnText;
    [SerializeField] private Button m_endTurnButton;
    [SerializeField] private Button m_undoButton;

    private Action m_onEndTurnClicked;
    private Action m_onUndoClicked;

    private void Awake()
    {
        if (m_endTurnButton != null)
        {
            m_endTurnButton.onClick.AddListener(HandleEndTurnClicked);
        }

        if (m_undoButton != null)
        {
            m_undoButton.onClick.AddListener(HandleUndoClicked);
        }
    }

    private void OnDestroy()
    {
        if (m_endTurnButton != null)
        {
            m_endTurnButton.onClick.RemoveListener(HandleEndTurnClicked);
        }

        if (m_undoButton != null)
        {
            m_undoButton.onClick.RemoveListener(HandleUndoClicked);
        }
    }

    public void Initialize(Action _onEndTurnClicked, Action _onUndoClicked)
    {
        m_onEndTurnClicked = _onEndTurnClicked;
        m_onUndoClicked = _onUndoClicked;
    }

    public void Refresh(PlayerSide _currentPlayer, int _turnCount, bool _canEndTurn, bool _canUndo)
    {
        if (m_turnText != null)
        {
            m_turnText.text = $"Turn {_turnCount} - {_currentPlayer}";
        }

        if (m_endTurnButton != null)
        {
            m_endTurnButton.gameObject.SetActive(_canEndTurn);
        }

        if (m_undoButton != null)
        {
            m_undoButton.gameObject.SetActive(_canUndo);
        }
    }

    private void HandleEndTurnClicked()
    {
        m_onEndTurnClicked?.Invoke();
    }

    private void HandleUndoClicked()
    {
        m_onUndoClicked?.Invoke();
    }
}
