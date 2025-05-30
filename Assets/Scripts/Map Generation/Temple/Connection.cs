using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection
{
    public Vector2Int Position { get; set; }
    public TempleRoom ParentRoom { get; set; }
    public List<Bond> Bonds { get; set; }

    public Connection(Vector2Int position, TempleRoom parentRoom) 
    {
        Position = position;
        ParentRoom = parentRoom;
        Bonds = new List<Bond>();
    }

    public void AddBond(Vector2Int direction, Connection connection)
    {
        Bonds.Add(new Bond(direction, connection));
    }

    public void AddBond(Bond bond)
    {
        Bonds.Add(bond);
    }

}
