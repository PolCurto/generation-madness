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
    public  List<Connection> Connections { get; set; }
    public List<Vector2Int> GridPositions { get; set; }

    public enum TempleRoomType
    {
        Start = 0,
        Normal = 1,
        Boss = 2,
        Treasure = 3,
        Character = 4,
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
    }

    public void AddConnectedRoom(TempleRoom newRoom)
    {
        if (_connectedRooms.Contains(newRoom) || newRoom == this) return;

        _connectedRooms.Add(newRoom);
    }

    public int Depth { get { return _depth; } set { _depth = value; } }
    public Vector2Int Position { get { return _position; } set { _position = value; } }
    public TempleRoomType Type { get { return _type; } set { _type = value; } }
    public List<TempleRoom> ConnectedRooms => _connectedRooms;
}