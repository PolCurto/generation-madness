using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPos
{
    public int Depth { get; set; }
    public Vector2Int Position { get; set; }

    public GridPos(int depth, Vector2Int position)
    {
        Depth = depth;
        Position = position;
    }
}
