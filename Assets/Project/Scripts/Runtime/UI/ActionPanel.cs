using System;
using UnityEngine;
using UnityEngine.UI;

public class ActionPanel : MonoBehaviour
{
    [SerializeField] private GameObject m_root;
    [SerializeField] private Button m_waitButton;
    [SerializeField] private Button m_defendButton;

    private Action m_onWaitClicked;
    private Action m_onDefendClicked;

    private void Awake()
    {
        if (m_waitButton != null)
        {
            m_waitButton.onClick.AddListener(HandleWaitClicked);
        }

        if (m_defendButton != null)
        {
            m_defendButton.onClick.AddListener(HandleDefendClicked);
        }

        Hide();
    }

    private void OnDestroy()
    {
        if (m_waitButton != null)
        {
            m_waitButton.onClick.RemoveListener(HandleWaitClicked);
        }

        if (m_defendButton != null)
        {
            m_defendButton.onClick.RemoveListener(HandleDefendClicked);
        }
    }

    public void Initialize(Action _onWaitClicked, Action _onDefendClicked)
    {
        m_onWaitClicked = _onWaitClicked;
        m_onDefendClicked = _onDefendClicked;
    }

    public void Show()
    {
        if (m_root != null)
        {
            m_root.SetActive(true);
        }
    }

    public void Hide()
    {
        if (m_root != null)
        {
            m_root.SetActive(false);
        }
    }

    private void HandleWaitClicked()
    {
        m_onWaitClicked?.Invoke();
    }

    private void HandleDefendClicked()
    {
        m_onDefendClicked?.Invoke();
    }
}
