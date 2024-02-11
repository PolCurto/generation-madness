using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    private int _depth;
    private RoomType _type;
    private Vector2 _position;
    private List<Room> _connectedRooms;
    private GameObject _sceneRoom;
    private int _width;
    private int _height;

    private List<Corridor> _corridors;

    public enum RoomType
    {
        Start = 0,
        Normal = 1,
        Boss = 2,
        Treasure = 3,
        Loop = 4,
        Character = 5,
        Checkpoint = 6,
        KeyRoom = 7
    }

    public Room (RoomType type, Vector2 position)
    {
        _depth = 100;
        _type = type;
        _position = position;
        _connectedRooms = new List<Room>();
        _corridors = new List<Corridor>();
    }

    public void AddConnectedRoom(Room newRoom)
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
    public RoomType Type { get { return _type; } set { _type = value; } }
    public GameObject SceneRoom => _sceneRoom; 
    public List<Room> ConnectedRooms => _connectedRooms;
    public List<Corridor> Corridors => _corridors;
}
