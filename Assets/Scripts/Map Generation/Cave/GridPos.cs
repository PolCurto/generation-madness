using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPos
{
    public int Depth { get; set; }
    public Vector2Int CellPosition { get; set; }
    public Vector2Int WorldPosition { get; set; }
    public List<GridPos> Neighbours { get; set; }
    public GridPos CameFrom { get; set; }
    public int GCost { get; set; }
    public int HCost { get; set; }
    public int FCost { get; set; }
    public bool Collapsed { get; set; }
    public List<Node> PossibleNodes { get; set; }

    public GridPos(Vector2Int position)
    {
        CellPosition = position;
        Neighbours = new List<GridPos>();
        GCost = int.MaxValue;
    }

    public void CalculateFCost()
    {
        FCost = GCost + HCost;
    }

    public bool IsNearWall()
    {
        if (Neighbours.Count < 8) return true;
        else return false;
    }

    public bool IsCorner()
    {
        int counter = 0;

        foreach (GridPos neighbour in Neighbours)
        {
            Vector2Int distance = CellPosition - neighbour.CellPosition;
            if (Mathf.Abs(distance.x) + Mathf.Abs(distance.y) <= 1)
            {
                counter++;
            }

            if (counter >= 3)
            {
                
                return false;
            }
        }
        return true;
    }

    public bool HasNeighbourInPositions(Vector2Int[] positions)
    {
        int counter = 0;

        foreach (Vector2Int offset in positions)
        {
            foreach (GridPos neighbour in Neighbours)
            {
                Vector2Int distance = neighbour.CellPosition - CellPosition;
                if (distance == offset)
                {
                    counter++;
                }
            }
        }
        return counter == positions.Length;
    }
}