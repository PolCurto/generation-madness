using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection
{
    private Vector2Int _gridPosition;
    private Dictionary<Vector2Int, TempleRoom> _connections;
    
    public Connection(Vector2Int gridPosition) 
    { 
        _gridPosition = gridPosition;
    }

    public void AddConnection(Vector2Int direction, TempleRoom room)
    {
        _connections.Add(direction, room);
    }

}
