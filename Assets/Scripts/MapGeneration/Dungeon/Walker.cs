using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walker
{
    public Vector2Int Position { get; set; }
    public Vector2Int PreviousPosition { get; set; }

    public int TimeToLive { get; set; }


    public Walker(Vector2Int position, int timeToLive)
    {
        Position = position;
        PreviousPosition = position;
        TimeToLive = timeToLive;
    }
}
