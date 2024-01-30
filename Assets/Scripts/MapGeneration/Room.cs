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

    public enum RoomType
    {
        Start = 0,
        Normal = 1,
        Boss = 2
    }

    public Room (int depth, RoomType type, Vector2 position)
    {
        _depth = depth;
        _type = type;
        _position = position;
        _connectedRooms = new List<Room>();
    }

    public void AddConnectedRoom(Room newRoom)
    {
        _connectedRooms.Add(newRoom);
    }

    public void AddSceneRoom(GameObject sceneRoom)
    {
        _sceneRoom = sceneRoom;
    }

    public Vector2 Positon { get { return _position; } set { _position = value; } }
    public RoomType Type { get { return _type; } set { _type = value; } }
    public List<Room> ConnectedRooms => _connectedRooms;
}
