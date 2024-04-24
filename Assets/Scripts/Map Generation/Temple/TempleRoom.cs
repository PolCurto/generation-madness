using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TempleRoom
{
    private int _depth;
    private TempleRoomType _type;
    private Vector2Int _position;
    private List<TempleRoom> _connectedRooms;

    public GameObject SceneRoom { get; set; }
    public List<Connection> Connections { get; set; }
    public List<Vector2Int> GridPositions { get; set; }
    public bool Completed { get; set; }

    public Dictionary<Vector2, GameObject> Items { get; set; }
    public Dictionary<Vector2, GameObject> Enemies { get; set; }

    public enum TempleRoomType
    {
        Start = 0,
        Normal = 1,
        Boss = 2,
        Treasure = 3,
        Weapon = 4,
        KeyRoom = 5,
        LongHorizontal = 6,
        LongVertical = 7,
        Big = 8
    }

    public TempleRoom(TempleRoomType type, Vector2Int position)
    {
        _depth = 100;
        _type = type;
        _position = position;
        _connectedRooms = new List<TempleRoom>();
        GridPositions = new List<Vector2Int>();
        Connections = new List<Connection>();
        Items = new Dictionary<Vector2, GameObject>();
        Enemies = new Dictionary<Vector2, GameObject>();
    }

    public void AddConnectedRoom(TempleRoom newRoom)
    {
        if (_connectedRooms.Contains(newRoom) || newRoom == this) return;

        _connectedRooms.Add(newRoom);
    }

    public void AddConnection(Connection connection)
    {
        if (!TryGetConnectionInPosition(connection.Position, out _))
        {
            Connections.Add(connection);
        }
    }

    public bool TryGetConnectionInPosition(Vector2Int position, out Connection connection)
    {
        connection = null;

        foreach(Connection conn in Connections)
        {
            connection = conn;
            if (conn.Position == position)
            {
                return true;
            }
        }

        return false;        
    }

    public int Depth { get { return _depth; } set { _depth = value; } }
    public Vector2Int Position { get { return _position; } set { _position = value; } }
    public TempleRoomType Type { get { return _type; } set { _type = value; } }
    public List<TempleRoom> ConnectedRooms => _connectedRooms;
}