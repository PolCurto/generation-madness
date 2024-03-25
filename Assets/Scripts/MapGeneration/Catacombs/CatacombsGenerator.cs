using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class CatacombsGenerator : MonoBehaviour
{
    #region Global Variables
    [SerializeField] private TilesController _tilesController;
    [SerializeField] private GameObject _player;

    [Header("Rooms")]
    [SerializeField] private GameObject _testingRoom;
    [SerializeField] private GameObject _startRoom;
    [SerializeField] private GameObject _treasureRoom;
    [SerializeField] private GameObject _characterRoom;
    [SerializeField] private GameObject _keyRoom;
    [SerializeField] private GameObject _bossRoom;
    [SerializeField] private GameObject[] _normalRooms;

    [Header("Floor Params")]
    [SerializeField] private int _maxGenerationIterations;
    [SerializeField] private int _maxRooms;
    [SerializeField] private int _gridWidth;
    [SerializeField] private int _gridHeight;
    [Range(0, 2)]
    [SerializeField] private int _adjacentRooms;
    [SerializeField] private int _minDeadEnds;
    [SerializeField] private int _maxDeadEnds;
    [SerializeField] private int _minDepth;

    [Header("Walkers Params")]
    [SerializeField] private int _walkersTimeToLive;

    [Header("Generation Params")]
    [SerializeField] private TileBase _wallTile;

    private Vector2Int _startPosition;
    private Walker _walker;
    private CatacombsRoom[,] _floorGrid;
    private List<CatacombsRoom> _rooms;
    private List<CatacombsRoom> _deadEnds;

    private int _iterations;
    #endregion

    void Awake()
    {
        _startPosition = new Vector2Int(_gridHeight / 2, _gridWidth / 2);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GenerateFloor();
            RenderFloorPrototype();
        }
    }

    /// <summary>
    /// Generates the rooms distribution within the floor   
    /// </summary>
    private void GenerateFloor()
    {
        _floorGrid = new CatacombsRoom[_gridHeight, _gridWidth];
        _rooms = new List<CatacombsRoom>();
        _deadEnds = new List<CatacombsRoom>();

        _walker = new Walker(_startPosition, _walkersTimeToLive);

        CatacombsRoom startRoom = new CatacombsRoom(CatacombsRoom.CatacombsRoomType.Start, _walker.Position);
        _floorGrid[_walker.Position.x, _walker.Position.y] = startRoom;
        _rooms.Add(startRoom);

        int roomsLeft = _maxRooms;

        while (roomsLeft > 0)
        {
            Debug.Log("Walker position: " + _walker.Position + " and rooms left: " + roomsLeft);

            if (_walker.TimeToLive <= 0)
            {
                // Resets the walker to the start (no need to create a new one)
                _walker.Position = _startPosition;
                _walker.TimeToLive = _walkersTimeToLive;
            }

            MoveWalker();

            if (CheckCurrentRoom())
            {
                CreateRoom();
                roomsLeft--;
            }
        }

        GetFloorStructure();
    }

    /// <summary>
    /// Gets the floor depth and dead ends
    /// </summary>
    private void GetFloorStructure()
    {
        _rooms[0].Depth = 0;

        foreach (CatacombsRoom room in _rooms)
        {
            // Adds all the rooms that are in a extreme of the map
            if (room.ConnectedRooms.Count < 2 && room.Type != CatacombsRoom.CatacombsRoomType.Start)
            {
                _deadEnds.Add(room);
            }

            // Sets the depth of each generated room
            foreach (CatacombsRoom connectedRoom in room.ConnectedRooms)
            {
                if (connectedRoom.Depth > room.Depth + 1)
                {
                    connectedRoom.Depth = room.Depth + 1;
                }
            }
        }

        // If there are not enough dead ends generates the floor again
        if (_deadEnds.Count < 4 || _deadEnds.Count > _maxDeadEnds)
        {
            GenerateFloor();
            return;
        }

        // Sort the dead ends by their depth
        _deadEnds.Sort((r1, r2) => r1.Depth.CompareTo(r2.Depth));

        // If there is no room deep enough generates the floor again
        if (_deadEnds[^1].Depth < _minDepth && _iterations < _maxGenerationIterations)
        {
            _iterations++;
            GenerateFloor();
            return;
        }


        SetSpecialRooms();
    }

    /// <summary>
    /// Stablishes the special rooms of the floor
    /// </summary>
    private void SetSpecialRooms()
    {
        // Creates the boss room as far from the start as possible
        CatacombsRoom bossRoom = _deadEnds[^1];
        bossRoom.Type = CatacombsRoom.CatacombsRoomType.Boss;

        // Sets the key room as far from the boss room as possible
        CatacombsRoom keyRoom = bossRoom;
        float prevDistance = 0;

        foreach (CatacombsRoom deadEnd in _deadEnds)
        {
            if (deadEnd.Type == CatacombsRoom.CatacombsRoomType.Normal)
            {
                float currentDistance = Vector2Int.Distance(deadEnd.Position, bossRoom.Position);
                if (currentDistance > prevDistance)
                {
                    prevDistance = currentDistance;
                    keyRoom = deadEnd;
                }
            }
        }
        keyRoom.Type = CatacombsRoom.CatacombsRoomType.KeyRoom;

        // Creates the character room
        foreach (CatacombsRoom deadEnd in _deadEnds)
        {
            if (deadEnd.Type == CatacombsRoom.CatacombsRoomType.Normal)
            {
                deadEnd.Type = CatacombsRoom.CatacombsRoomType.Character;
                break;
            }
        }

        // Creates the treasure room
        foreach (CatacombsRoom deadEnd in _deadEnds)
        {
            if (deadEnd.Type == CatacombsRoom.CatacombsRoomType.Normal)
            {
                deadEnd.Type = CatacombsRoom.CatacombsRoomType.Treasure;
                break;
            }
        }
    }

    /// <summary>
    /// Creates a new Room and its connections
    /// </summary>
    private void CreateRoom()
    {
        CatacombsRoom newRoom = new CatacombsRoom(CatacombsRoom.CatacombsRoomType.Normal, _walker.Position);
        CatacombsRoom previousRoom = _floorGrid[_walker.PreviousPosition.x, _walker.PreviousPosition.y];

        newRoom.AddConnectedRoom(previousRoom);
        previousRoom.AddConnectedRoom(newRoom);

        _floorGrid[_walker.Position.x, _walker.Position.y] = newRoom;
        _rooms.Add(newRoom);
    }

    /// <summary>
    /// Moves the walker in the floor grid
    /// </summary>
    /// <returns>The new walker's position</returns>
    private void MoveWalker()
    {
        _walker.PreviousPosition = _walker.Position;
        _walker.TimeToLive -= 1;

        Vector2Int movement = Vector2Int.zero;

        if (Random.Range(0f, 1f) > 0.5f)
        {
            //Move horizontal
            if (Random.Range(0f, 1f) > 0.5f) movement.x = 1;
            else movement.x = -1;
        }
        else
        {
            // Move vertical
            if (Random.Range(0f, 1f) > 0.5f) movement.y = 1;
            else movement.y = -1;
        }

        _walker.Position += movement;
    }

    /// <summary>
    /// Checks if the current position is valid to create a new Room
    /// </summary>
    private bool CheckCurrentRoom()
    {
        if (_floorGrid[_walker.Position.x, _walker.Position.y] == null)
        {
            int emptyRooms = 0;

            if (_floorGrid[_walker.Position.x + 1, _walker.Position.y] == null) emptyRooms++;
            if (_floorGrid[_walker.Position.x - 1, _walker.Position.y] == null) emptyRooms++;
            if (_floorGrid[_walker.Position.x, _walker.Position.y + 1] == null) emptyRooms++;
            if (_floorGrid[_walker.Position.x, _walker.Position.y - 1] == null) emptyRooms++;

            if (emptyRooms > _adjacentRooms)
            {
                return true;
            }
            else
            {
                _walker.Position = _walker.PreviousPosition;
            }
        }

        return false;
    }

    /// <summary>
    /// Renders the floor with the selected rooms once the loop has ended
    /// </summary>
    private void RenderFloorPrototype()
    {
        foreach (CatacombsRoom room in _rooms)
        {
            // Center to 0
            room.Position -= _startPosition;
            GameObject newRoom;

            Vector3 position = new Vector3(room.Position.x, room.Position.y);

            switch (room.Type)
            {
                case CatacombsRoom.CatacombsRoomType.Start:
                    newRoom = Instantiate(_testingRoom, position, Quaternion.identity);
                    newRoom.GetComponent<SpriteRenderer>().color = new Color(1, 1, 0);
                    room.AddSceneRoom(newRoom);
                    break;

                case CatacombsRoom.CatacombsRoomType.Normal:
                    newRoom = Instantiate(_testingRoom, position, Quaternion.identity);
                    float value = room.Depth * 40;
                    newRoom.GetComponent<SpriteRenderer>().color = new Color((255 - value) / 255, (255 - value) / 255, (255 - value) / 255);
                    room.AddSceneRoom(newRoom);
                    break;

                case CatacombsRoom.CatacombsRoomType.Treasure:
                    newRoom = Instantiate(_testingRoom, position, Quaternion.identity);
                    newRoom.GetComponent<SpriteRenderer>().color = new Color(1, 200f / 255, 0);
                    room.AddSceneRoom(newRoom);
                    break;

                case CatacombsRoom.CatacombsRoomType.Character:
                    newRoom = Instantiate(_testingRoom, position, Quaternion.identity);
                    newRoom.GetComponent<SpriteRenderer>().color = new Color(0, 0, 1);
                    room.AddSceneRoom(newRoom);
                    break;

                case CatacombsRoom.CatacombsRoomType.KeyRoom:
                    newRoom = Instantiate(_testingRoom, position, Quaternion.identity);
                    newRoom.GetComponent<SpriteRenderer>().color = new Color(0, 1, 1);
                    room.AddSceneRoom(newRoom);
                    break;

                case CatacombsRoom.CatacombsRoomType.Boss:
                    newRoom = Instantiate(_testingRoom, position, Quaternion.identity);
                    newRoom.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0);
                    room.AddSceneRoom(newRoom);
                    break;
            }
        }
    }
}
