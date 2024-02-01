using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corridor
{
    private Room _originRoom;
    private Room _destinationRoom;

    private List<Vector2> _spacePoints;

    public Corridor(Room originRoom, Room destinationRoom)
    {
        _originRoom = originRoom;
        _destinationRoom = destinationRoom;
        _spacePoints = new List<Vector2>
        {
            originRoom.SceneRoom.transform.position
        };
    }

    public void AddNewPosition(Vector2 position)
    {
        _spacePoints.Add(position);
    }

    public Room OriginRoom => _originRoom;
    public Room DestinationRoom => _destinationRoom;

    public List<Vector2> SpacePoints { get { return _spacePoints; } set { _spacePoints = value; } }

}
