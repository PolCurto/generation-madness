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
}