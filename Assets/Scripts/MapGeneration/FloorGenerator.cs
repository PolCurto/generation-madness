using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class FloorGenerator : MonoBehaviour
{
    #region Global Variables
    [SerializeField] private GameObject _startRoom;
    [SerializeField] private GameObject _bossRoom;
    [SerializeField] private GameObject[] _normalRooms;

    [Header ("Floor Params")]
    [SerializeField] private int _maxRooms;
    [SerializeField] private int _gridWidth;
    [SerializeField] private int _gridHeight;
    [SerializeField] private int _movementScalar;
    [Range(0, 2)]
    [SerializeField] private int _adjacentRooms;
    [Range(0f, 1f)]
    [SerializeField] private float _connectionChance;
    [SerializeField] private int _maxDeadEnds;
    [SerializeField] private int _minDepth;

    [Header("Walkers Params")]
    //[SerializeField] private int _numWalkers;
    [SerializeField] private int _walkersTimeToLive;

    [Header("Generation Params")]
    [SerializeField] private float _pullTime;
    [SerializeField] private float _pullSpeed;

    private Vector2Int _startPosition;
    private Walker walker;
    private Room[,] _floorGrid;
    private List<Room> _roomsList;
    private List<Room> _deadEnds;
    private List<Corridor> _corridors;
    #endregion

    #region Unity Methods
    void Start()
    {
        _startPosition = new Vector2Int(_gridHeight / 2, _gridWidth / 2);
        /*
        GenerateFloor();
        RenderFloor();
        GenerateCorridors();
        //StartCoroutine(PullRoomsTogether());
        */
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) 
        {
            GenerateFloor();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            RenderFloor();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            GenerateCorridors();
        }

        if (_corridors != null)
        {
            foreach (Corridor corridor in _corridors)
            {
                for (int i = 0; i < corridor.SpacePoints.Count - 1; i++)
                {
                    Debug.DrawLine(corridor.SpacePoints[i], corridor.SpacePoints[i + 1]);
                }
            }
        }
        

        /*
        foreach (Room room in _roomsList)
        {
            foreach (Room connectedRoom in room.ConnectedRooms)
            {
                Debug.DrawLine(room.SceneRoom.transform.position, connectedRoom.SceneRoom.transform.position);
            }  
        }
        */
    }
    #endregion


    #region Floor Logic
    /// <summary>
    /// Generates the rooms distribution within the floor
    /// </summary>
    private void GenerateFloor()
    {
        _floorGrid = new Room[_gridHeight, _gridWidth];
        _roomsList = new List<Room>();
        _deadEnds = new List<Room>();
        _corridors = new List<Corridor>();

        walker = new Walker(_startPosition, _walkersTimeToLive);

        Room startRoom = new Room(Room.RoomType.Start, walker.Position);
        _floorGrid[walker.Position.x, walker.Position.y] = startRoom;
        _roomsList.Add(startRoom);

        int roomsLeft = _maxRooms;

        while (roomsLeft > 0)
        {
            //Debug.Log("Walker position: " + walker.Position + " and rooms left: " + roomsLeft);

            if (walker.TimeToLive <= 0)
            {
                // Resets the walker to the start (no need to create a new one)
                walker.Position = _startPosition;
                walker.TimeToLive = _walkersTimeToLive;
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
        _roomsList[0].Depth = 0;

        foreach (Room room in _roomsList)
        {
            // Adds all the rooms that are in a extreme of the map
            if (room.ConnectedRooms.Count < 2 && room.Type != Room.RoomType.Start)
            {
                _deadEnds.Add(room);
            }

            // Sets the depth of each generated room
            foreach (Room connectedRoom in room.ConnectedRooms)
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
        if (_deadEnds[^1].Depth < _minDepth)
        {
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
        // Checks for loops
        foreach (Room room in _roomsList)
        {
            if (room.ConnectedRooms.Count > 1)
            {
                int shallowerRooms = 0;

                foreach (Room connectedRoom in room.ConnectedRooms)
                {
                    if (connectedRoom.Depth < room.Depth) shallowerRooms++;
                }

                if (shallowerRooms >= 2)
                {
                    room.Type = Room.RoomType.Loop;
                }
            }
        }
        
        // Creates the boss room as far from the start as possible
        CreateBossRoom();
        Room bossRoom = _deadEnds[^1];
        bossRoom.Type = Room.RoomType.Boss;

        // Sets the key room as far from the boss room as possible
        Room keyRoom = bossRoom;

        foreach (Room deadEnd in _deadEnds)
        {
            if (deadEnd.Type == Room.RoomType.Normal)
            {
                float prevDistance = Mathf.Abs(keyRoom.Position.x - bossRoom.Position.x) + Mathf.Abs(keyRoom.Position.y - bossRoom.Position.y);
                float currentDistance = Mathf.Abs(deadEnd.Position.x - bossRoom.Position.x) + Mathf.Abs(deadEnd.Position.y - bossRoom.Position.y);
                if (currentDistance > prevDistance)
                {
                    keyRoom = deadEnd;
                }
            }
        }
        keyRoom.Type = Room.RoomType.KeyRoom;

        // Creates the charatcer room
        foreach (Room deadEnd in _deadEnds)
        {
            if (deadEnd.Type == Room.RoomType.Normal)
            {
                deadEnd.Type = Room.RoomType.Character;
                break;
            }
        }

        // Creates the treasure room
        foreach (Room deadEnd in _deadEnds)
        {
            if (deadEnd.Type == Room.RoomType.Normal)
            {
                deadEnd.Type = Room.RoomType.Treasure;
                break;
            }
        }
    }

    /// <summary>
    /// Creates a new Room and its connections
    /// </summary>
    private void CreateRoom()
    {
        Room newRoom = new Room(Room.RoomType.Normal, walker.Position);
        Room previousRoom = _floorGrid[walker.PreviousPosition.x, walker.PreviousPosition.y];

        newRoom.AddConnectedRoom(previousRoom);
        previousRoom.AddConnectedRoom(newRoom);

        _floorGrid[walker.Position.x, walker.Position.y] = newRoom;
        _roomsList.Add(newRoom);
    }

    /// <summary>
    /// Creates the boss room 
    /// </summary>
    private void CreateBossRoom()
    {
        Room checkpointRoom = _deadEnds[^1];
        checkpointRoom.Type = Room.RoomType.Checkpoint;

        walker.PreviousPosition = Vector2Int.RoundToInt(checkpointRoom.Position);
        walker.Position = Vector2Int.RoundToInt(checkpointRoom.Position + (checkpointRoom.Position - checkpointRoom.ConnectedRooms[0].Position));

        CreateRoom();
        _deadEnds.RemoveAt(_deadEnds.Count - 1);
        _deadEnds.Add(_roomsList[^1]);
    }

    /// <summary>
    /// Moves the walker in the floor grid
    /// </summary>
    /// <returns>The new walker's position</returns>
    private void MoveWalker()
    {
        walker.PreviousPosition = walker.Position;
        walker.TimeToLive -= 1;

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

        walker.Position += movement;
    }

    /// <summary>
    /// Checks if the current position is valid to create a new Room
    /// </summary>
    private bool CheckCurrentRoom()
    {
        if (_floorGrid[walker.Position.x, walker.Position.y] == null)
        {
            int emptyRooms = 0;

            if (_floorGrid[walker.Position.x + 1, walker.Position.y] == null) emptyRooms++;
            if (_floorGrid[walker.Position.x - 1, walker.Position.y] == null) emptyRooms++;
            if (_floorGrid[walker.Position.x, walker.Position.y + 1] == null) emptyRooms++;
            if (_floorGrid[walker.Position.x, walker.Position.y - 1] == null) emptyRooms++;

            if (emptyRooms > _adjacentRooms)
            {
                return true;
            }
            else
            {
                walker.Position = walker.PreviousPosition;
            }
        }
        else
        {
            if (Random.Range(0f, 1f) < _connectionChance)
            {
                Room currentRoom = _floorGrid[walker.Position.x, walker.Position.y];
                Room previousRoom = _floorGrid[walker.PreviousPosition.x, walker.PreviousPosition.y];

                currentRoom.AddConnectedRoom(previousRoom);
                previousRoom.AddConnectedRoom(currentRoom);
            }
        }

        return false;
    }
    #endregion

    #region Floor Visuals
    /// <summary>
    /// Renders the floor with the selected rooms once the loop has ended
    /// </summary>
    private void RenderFloor()
    {
        foreach(Room room in _roomsList)
        {
            room.Position *= _movementScalar;

            // Center to 0
            room.Position -= _startPosition * _movementScalar;
            GameObject newRoom;

            switch (room.Type)
            {
                case Room.RoomType.Start:
                    newRoom = Instantiate(_startRoom, room.Position, Quaternion.identity);
                    room.AddSceneRoom(newRoom);
                    break;

                case Room.RoomType.Normal:
                    newRoom = Instantiate(_normalRooms[0], room.Position, Quaternion.identity);

                    if (Random.Range(0f, 1f) < 0.75f)
                    {
                        Vector3 var = newRoom.transform.localScale;
                        var.x *= Random.Range(1, 3);
                        var.y *= Random.Range(1, 3);
                        newRoom.transform.localScale = var;
                    }
                    float value = room.Depth * 40;
                    newRoom.GetComponent<SpriteRenderer>().color = new Color((255 - value) / 255, (255 - value) / 255, (255 - value) / 255);

                    room.AddSceneRoom(newRoom);
                    break;

                case Room.RoomType.Treasure:
                    newRoom = Instantiate(_normalRooms[0], room.Position, Quaternion.identity);
                    newRoom.GetComponent<SpriteRenderer>().color = new Color(1, 200f/255, 0);
                    room.AddSceneRoom(newRoom);
                    break;

                case Room.RoomType.Character:
                    newRoom = Instantiate(_normalRooms[0], room.Position, Quaternion.identity);
                    newRoom.GetComponent<SpriteRenderer>().color = new Color(0, 0, 1);
                    room.AddSceneRoom(newRoom);
                    break;

                case Room.RoomType.Loop:
                    newRoom = Instantiate(_normalRooms[0], room.Position, Quaternion.identity);
                    newRoom.GetComponent<SpriteRenderer>().color = new Color(0, 1, 0);
                    room.AddSceneRoom(newRoom);
                    break;

                case Room.RoomType.KeyRoom:
                    newRoom = Instantiate(_normalRooms[0], room.Position, Quaternion.identity);
                    newRoom.GetComponent<SpriteRenderer>().color = new Color(0, 1, 1);
                    room.AddSceneRoom(newRoom);
                    break;

                case Room.RoomType.Checkpoint:
                    newRoom = Instantiate(_normalRooms[0], room.Position, Quaternion.identity);
                    newRoom.GetComponent<SpriteRenderer>().color = new Color(1, 0, 1);
                    room.AddSceneRoom(newRoom);
                    break;

                case Room.RoomType.Boss:
                    newRoom = Instantiate(_normalRooms[0], room.Position, Quaternion.identity);
                    newRoom.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0);
                    room.AddSceneRoom(newRoom);
                    break;
            }
        }
    }

    /// <summary>
    /// Pulls all the rooms close to the starting room so it doesn't feel like they are in a grid
    /// </summary>
    /// <returns>IEnumerator</returns>
    private IEnumerator PullRoomsTogether()
    {
        float timer = 0;
        Vector2 velocity;
        List<Rigidbody2D> sceneRooms = new List<Rigidbody2D>();

        // Gets all the rooms rigidbodies
        for (int i = 0; i < _roomsList.Count; i++)
        {
            if (_roomsList[i].SceneRoom.TryGetComponent<Rigidbody2D>(out var rigidbody))
            {
                sceneRooms.Add(rigidbody);
            }
        }

        // Pulls the rooms to the center during a period of time
        while (timer < _pullTime)
        {
            yield return new WaitForFixedUpdate();

            timer += Time.deltaTime;

            for (int i = 0; i < sceneRooms.Count; i++)
            {
                velocity = Vector2.MoveTowards(sceneRooms[i].position, _roomsList[0].Position, _pullSpeed);
                sceneRooms[i].MovePosition(velocity);
            }
        }

        // Waits and stops the rooms from moving again
        yield return new WaitForSeconds(0.1f);

        foreach (Room room in _roomsList)
        {
            if (room.SceneRoom.TryGetComponent<Rigidbody2D>(out var rigidbody))
            {
                rigidbody.velocity = Vector2.zero;
            }
        }

        GenerateCorridors();
    }
    #endregion

    #region Corridors
    private void GenerateCorridors()
    {
        foreach (Room room in _roomsList)
        {
            foreach (Room connectedRoom in room.ConnectedRooms)
            {
                if (!CheckDuplicatedCorridors(room, connectedRoom))
                {
                    Vector2 roomPosition = room.SceneRoom.transform.position;
                    Vector2 connectedRoomPosition = connectedRoom.SceneRoom.transform.position;

                    Corridor corridor = new Corridor(room, connectedRoom);
                    Debug.Log("New corridor with origin room: " + room.Type + " | " + roomPosition + " and destination room: " + connectedRoom.Type + " | " + connectedRoomPosition);

                    float timer = 0;

                    while (timer < 100)
                    {
                        if (CheckConnectedRoomPosition(roomPosition, connectedRoomPosition))
                        {
                            corridor.AddNewPosition(new Vector2(connectedRoomPosition.x, corridor.SpacePoints[^1].y));
                        }
                        else
                        {
                            corridor.AddNewPosition(new Vector2(corridor.SpacePoints[^1].x, connectedRoomPosition.y));
                        }

                        if (corridor.SpacePoints[^1] == connectedRoomPosition)
                        {
                            break;
                        }

                        timer++;
                    }

                    room.AddCorridor(corridor);
                    connectedRoom.AddCorridor(corridor);
                    _corridors.Add(corridor);
                }
            }
        }
    }

    /// <summary>
    /// Checks for the corridors to avoid creating duplicated ones
    /// </summary>
    /// <returns>If the corridor already exists</returns>
    private bool CheckDuplicatedCorridors(Room room, Room connectedRoom)
    {
        // Avoids repeated corridors
        foreach (Room otherRoom in _roomsList)
        {
            foreach (Corridor corridor in otherRoom.Corridors)
            {
                if (corridor.OriginRoom == connectedRoom && corridor.DestinationRoom == room)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool CheckConnectedRoomPosition(Vector2 roomPosition, Vector2 connectedRoomPosition)
    {
        Vector2 positionDif = roomPosition - connectedRoomPosition;

        if (Mathf.Abs(positionDif.x) > Mathf.Abs(positionDif.y))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion
}