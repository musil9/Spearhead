using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text m_turnText;
    [SerializeField] private Button m_endTurnButton;

    private Action m_onEndTurnClicked;

    private void Awake()
    {
        if (m_endTurnButton != null)
        {
            m_endTurnButton.onClick.AddListener(HandleEndTurnClicked);
        }
    }

    private void OnDestroy()
    {
        if (m_endTurnButton != null)
        {
            m_endTurnButton.onClick.RemoveListener(HandleEndTurnClicked);
        }
    }

    public void Initialize(Action _onEndTurnClicked)
    {
        m_onEndTurnClicked = _onEndTurnClicked;
    }

    public void Refresh(PlayerSide _currentPlayer, int _turnCount, bool _canEndTurn)
    {
        if (m_turnText != null)
        {
            m_turnText.text = $"Turn {_turnCount} - {_currentPlayer}";
        }

        if (m_endTurnButton != null)
        {
            m_endTurnButton.gameObject.SetActive(_canEndTurn);
        }
    }

    private void HandleEndTurnClicked()
    {
        m_onEndTurnClicked?.Invoke();
    }
}
