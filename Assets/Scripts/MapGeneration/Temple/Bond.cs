using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bond
{
    public Vector2Int Direction { get; private set; }
    public Connection Connection { get; private set; }
    public Bond LinkedBond { get; set; }
    public DoorController DoorController { get; set; }

    public Bond(Vector2Int direction, Connection connection)
    {
        Direction = direction;
        Connection = connection;
    }
}
