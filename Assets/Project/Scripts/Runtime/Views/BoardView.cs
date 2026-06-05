using System.Collections.Generic;
using UnityEngine;

public sealed class BoardView : MonoBehaviour
{
    [SerializeField] private TileView m_tilePrefab;
    [SerializeField] private Transform m_tileRoot;

    private readonly Dictionary<Vector2Int, TileView> m_tileViews = new();

    public void Initialize(BoardModel _boardModel)
    {
        foreach (var tileModel in _boardModel.GetAllTiles())
        {
            var tileView = Instantiate(m_tilePrefab, m_tileRoot);
            tileView.Initialize(tileModel.Position);

            m_tileViews[tileModel.Position] = tileView;
        }
    }

    public TileView GetTileView(Vector2Int _position)
    {
        return m_tileViews.TryGetValue(_position, out var tileView)
            ? tileView
            : null;
    }

    public void ClearAllHover()
    {
        foreach (var tileView in m_tileViews.Values)
        {
            tileView.SetDefault();
        }
    }
}