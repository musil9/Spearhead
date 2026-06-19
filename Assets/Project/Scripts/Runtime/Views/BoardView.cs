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

    public void ShowMovableTiles(IReadOnlyList<Vector2Int> _positions)
    {
        ClearHighlights();

        foreach (var position in _positions)
        {
            var tileView = GetTileView(position);

            if (tileView == null)
                continue;

            tileView.SetMovable();
        }
    }

    public void ShowBattleAreas(IReadOnlyList<BattleArea> _battleAreas)
    {
        ClearBattleAreas();

        foreach (BattleArea battleArea in _battleAreas)
        {
            foreach (Vector2Int tilePosition in battleArea.Tiles)
            {
                TileView tileView = GetTileView(tilePosition);

                if (tileView == null)
                    continue;

                tileView.SetBattleArea(true);
            }
        }
    }

    public void ClearBattleAreas()
    {
        foreach (TileView tileView in m_tileViews.Values)
        {
            tileView.SetBattleArea(false);
        }
    }

    public void ShowBattlePreviewAreas(IReadOnlyList<BattleArea> _battleAreas)
    {
        ClearBattlePreviewAreas();

        foreach (BattleArea battleArea in _battleAreas)
        {
            foreach (Vector2Int tilePosition in battleArea.Tiles)
            {
                TileView tileView = GetTileView(tilePosition);

                if (tileView == null)
                    continue;

                tileView.SetBattlePreview(true);
            }
        }
    }

    public void ClearBattlePreviewAreas()
    {
        foreach (TileView tileView in m_tileViews.Values)
        {
            tileView.SetBattlePreview(false);
        }
    }

    public void ClearHighlights()
    {
        foreach (var tileView in m_tileViews.Values)
        {
            tileView.SetDefault();
        }
    }

    public void RestoreHoverVisual(TileView tileView)
    {
        if (tileView == null)
            return;

        tileView.RestoreVisual();
    }
}