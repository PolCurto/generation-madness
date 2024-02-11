using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corridor
{
    private Room _originRoom;
    private Room _destinationRoom;

    private List<Vector2Int> _positions;
    private List<bool> _orientation;

    public Corridor(Room originRoom, Room destinationRoom)
    {
        _originRoom = originRoom;
        _destinationRoom = destinationRoom;
        _positions = new List<Vector2Int>
        {
            Vector2Int.RoundToInt(originRoom.Position)
        };
        _orientation = new List<bool>
        {
            true
        };

    }

    public void AddNewPosition(Vector2Int position, bool horizontal)
    {
        _positions.Add(position);
        _orientation.Add(horizontal);
    }

    public void ResetPositions()
    {
        _positions = new List<Vector2Int>
        {
            Vector2Int.RoundToInt(_originRoom.Position)
        };
        _orientation = new List<bool>
        {
            true
        };
    }

    public Room OriginRoom => _originRoom;
    public Room DestinationRoom => _destinationRoom;

    public List<Vector2Int> Positions { get { return _positions; } set { _positions = value; } }
    public List<bool> Orientation { get { return _orientation; } set { _orientation = value; } }

}
