using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRoom
{
    private int _depth;
    private DungeonRoomType _type;
    private Vector2 _position;
    private List<DungeonRoom> _connectedRooms;
    private GameObject _sceneRoom;
    private int _width;
    private int _height;

    private List<Corridor> _corridors;

    public List<Vector2> EnemyPositions { get; set; }
    public List<GameObject> EnemyPool { get; set; }
    public List<int> EnemyTypeLimits { get; set; }

    public enum DungeonRoomType
    {
        Start = 0,
        Normal = 1,
        Boss = 2,
        Treasure = 3,
        Loop = 4,
        Weapon = 5,
        Checkpoint = 6,
        KeyRoom = 7
    }

    public DungeonRoom(DungeonRoomType type, Vector2 position)
    {
        _depth = 100;
        _type = type;
        _position = position;
        _connectedRooms = new List<DungeonRoom>();
        _corridors = new List<Corridor>();
        EnemyPositions = new List<Vector2>();
        EnemyPool = new List<GameObject>();
        EnemyTypeLimits = new List<int>();
    }

    public void AddConnectedRoom(DungeonRoom newRoom)
    {
        if (_connectedRooms.Contains(newRoom)) return;

        _connectedRooms.Add(newRoom);
    }

    public void AddSceneRoom(GameObject sceneRoom)
    {
        _sceneRoom = sceneRoom;
    }

    public void AddCorridor(Corridor corridor)
    {
        _corridors.Add(corridor);
    }

    public int Depth { get { return _depth; } set { _depth = value; } }
    public int Width { get { return _width; } set { _width = value; } }
    public int Height { get { return _height; } set { _height = value; } }
    public Vector2 Position { get { return _position; } set { _position = value; } }
    public DungeonRoomType Type { get { return _type; } set { _type = value; } }
    public GameObject SceneRoom => _sceneRoom; 
    public List<DungeonRoom> ConnectedRooms => _connectedRooms;
    public List<Corridor> Corridors => _corridors;
}
