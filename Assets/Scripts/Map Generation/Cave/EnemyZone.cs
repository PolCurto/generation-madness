using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyZone
{    public enum ZoneType
    {
        Easy = 0,
        Medium = 1,
        Hard = 2
    }

    public ZoneType Type { get; set; }
    public Vector2Int Position { get; set; }
    public int Area { get; set; }

    public EnemyZone(ZoneType type, Vector2Int position, int area)
    {
        Type = type;
        Position = position;
        Area = area;
    }
}