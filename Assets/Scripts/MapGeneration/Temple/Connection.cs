using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection
{
    public Vector2Int Position { get; set; }
    public Dictionary<Vector2Int, TempleRoom> Bonds { get; private set; }
    
    public Connection(Vector2Int position) 
    {
        Position = position;
        Bonds = new Dictionary<Vector2Int, TempleRoom>();
    }

    public void AddConnection(Vector2Int direction, TempleRoom room)
    {
        Bonds.Add(direction, room);
    }

}
