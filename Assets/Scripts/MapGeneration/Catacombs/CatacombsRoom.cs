using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static DungeonRoom;

public class CatacombsRoom
{
    private int _depth;
    private CatacombsRoomType _type;
    private Vector2Int _position;
    private List<CatacombsRoom> _connectedRooms;
    private GameObject _sceneRoom;

    public List<Vector2Int> OccupiedGridPositions { get; set; }

    public enum CatacombsRoomType
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

    public CatacombsRoom(CatacombsRoomType type, Vector2Int position)
    {
        _depth = 100;
        _type = type;
        _position = position;
        _connectedRooms = new List<CatacombsRoom>();
        OccupiedGridPositions = new List<Vector2Int>();
    }

    public void AddConnectedRoom(CatacombsRoom newRoom)
    {
        if (_connectedRooms.Contains(newRoom)) return;

        _connectedRooms.Add(newRoom);
    }

    public void AddSceneRoom(GameObject sceneRoom)
    {
        _sceneRoom = sceneRoom;
    }

    public int Depth { get { return _depth; } set { _depth = value; } }
    public Vector2Int Position { get { return _position; } set { _position = value; } }
    public CatacombsRoomType Type { get { return _type; } set { _type = value; } }
    public List<CatacombsRoom> ConnectedRooms => _connectedRooms;
}