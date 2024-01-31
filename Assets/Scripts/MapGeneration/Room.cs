using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    private int _depth;
    private RoomType _type;
    private Vector2 _position;
    private List<Room> _connectedRooms;
    public GameObject _sceneRoom;

    private bool _depthIsSet;

    public enum RoomType
    {
        Start = 0,
        Normal = 1,
        Boss = 2
    }

    public Room (RoomType type, Vector2 position)
    {
        _depth = 0;
        _type = type;
        _position = position;
        _connectedRooms = new List<Room>();
        _depthIsSet = false;
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

    public int Depth { get { return _depth; } set { _depth = value; } }
    public Vector2 Positon { get { return _position; } set { _position = value; } }
    public RoomType Type { get { return _type; } set { _type = value; } }
    public bool DepthIsSet { get { return _depthIsSet; } set { _depthIsSet = value; } }
    public List<Room> ConnectedRooms => _connectedRooms;
}
