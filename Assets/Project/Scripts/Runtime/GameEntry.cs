using System;
using UnityEngine;

public class GameEntry : MonoBehaviour
{
    [SerializeField] private BoardView m_boardView;

    private BoardModel m_boardModel;

    private void Start()
    {
        m_boardModel = new BoardModel(10, 10);
        m_boardView.Initialize(m_boardModel);

        Debug.Log("GameEntry Started.");
    }
}
