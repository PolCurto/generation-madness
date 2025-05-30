using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FloorGrid
{
    public List<GridPos> GridPositions { get; set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public int MaxDepth { get; set; }
    public GridPos StartPosition { get; set; }
    public GridPos BossPosition { get; set; }

    private Vector2Int[] surroundings = new Vector2Int[]
    {
        new Vector2Int (1, 0),      // Right
        new Vector2Int (1, -1),     // Right Down
        new Vector2Int (0, -1),     // Down
        new Vector2Int (-1, -1),    // Left Down
        new Vector2Int (-1, 0),     // Left
        new Vector2Int (-1, 1),     // Left Up
        new Vector2Int (0, 1),      // Up
        new Vector2Int (1, 1),      // Right Up
    };

    public FloorGrid (int width, int height)
    {
        GridPositions = new List<GridPos>();
        Width = width;
        Height = height;
    }

    public GridPos GetPosWithDepth(int depth)
    {
        foreach(GridPos pos in GridPositions)
        {
            if (pos.Depth == depth) return pos;
        }

        return null;
    }

    public bool TileExistsInCellPos(Vector2Int position)
    {
        foreach (GridPos pos in GridPositions)
        {
            if (pos.CellPosition == position)
                return true;
        }
        return false;
    }

    public bool TileExistsInWorldPos(Vector2Int position)
    {
        foreach (GridPos pos in GridPositions)
        {
            if (pos.WorldPosition == position)
                return true;
        }
        return false;
    }
    
    public GridPos GetGridPosFromWorld(Vector2Int worldPosition)
    {
        foreach (GridPos pos in GridPositions)
        {
            if (pos.WorldPosition == worldPosition)
                return pos;
        }
        Debug.LogWarning("There is not GridPos in that world position");
        return null;
    }

    public bool TryGetGridPosFromWorld(Vector2Int worldPosition, out GridPos gridPos)
    {
        foreach (GridPos pos in GridPositions)
        {
            if (pos.WorldPosition == worldPosition)
            {
                gridPos = pos;
                return true;
            }
        }
        gridPos = null;
        return false;
    }

    public GridPos GetGridPosFromCell(Vector2Int cellPosition)
    {
        foreach (GridPos pos in GridPositions)
        {
            if (pos.CellPosition == cellPosition)
                return pos;
        }
        Debug.LogWarning("There is not GridPos in that world position");
        return null;
    }

    public bool TryGetGridPosFromCell(Vector2Int cellPosition, out GridPos gridPos)
    {
        foreach (GridPos pos in GridPositions)
        {
            if (pos.CellPosition == cellPosition)
            {
                gridPos = pos;
                return true;
            }
        }
        gridPos = null;
        return false;
    }

    private List<GridPos> GetNeighbors(GridPos gridPos)
    {
        List<GridPos> neighbors = new List<GridPos>();

        foreach (Vector2Int offset in surroundings)
        {
            if (TileExistsInWorldPos(gridPos.WorldPosition + offset))
            {
                neighbors.Add(GetGridPosFromWorld(gridPos.WorldPosition + offset));
            }
        }
        return neighbors;
    }
}