using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Xml;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class FloorGenerator : MonoBehaviour
{
    #region Global Variables
    [SerializeField] private TilesController _tilesController;
    [SerializeField] private GameObject _player;
    [SerializeField] private bool _showConnections;

    [Header("Rooms")]
    [SerializeField] private GameObject _testingRoom;
    [SerializeField] private DungeonRoomData _startRoom;
    [SerializeField] private DungeonRoomData _treasureRoomData;
    [SerializeField] private DungeonRoomData _weaponRoomData;
    [SerializeField] private DungeonRoomData _managementRoom;
    [SerializeField] private DungeonRoomData _bossRoom;
    [SerializeField] private DungeonRoomData[] _normalRooms;

    [Header("Floor Params")]
    [SerializeField] private int _maxGenerationIterations;
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
    [SerializeField] private TileBase _wallTile;
    [SerializeField] private float _prefabChance;
    [SerializeField] private float _wallPrefabChance;

    [Header("Items & Weapons")]
    [SerializeField] private GameObject _passiveItemPrefab;
    [SerializeField] private ItemBase[] _itemsPool;
    [SerializeField] private GameObject _weaponPrefab;
    [SerializeField] private WeaponBase[] _weaponsPool;
    [SerializeField] private GameObject[] _decoration;
    [SerializeField] private GameObject[] _wallDecoration;

    private Vector2Int _startPosition;
    private Walker _walker;
    private DungeonRoom[,] _floorGrid;
    private List<DungeonRoom> _rooms;
    private List<DungeonRoom> _deadEnds;
    private List<Corridor> _corridors;

    // Special Rooms
    private DungeonRoom _treasureRoom;
    private DungeonRoom _weaponRoom;

    private int _iterations;
    #endregion

    #region Unity Methods
    void Start()
    {
        _startPosition = new Vector2Int(_gridHeight / 2, _gridWidth / 2);
        StartCoroutine(GenerateLevel());
    }

    private void Update()
    {
        /*
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
            StartCoroutine(PullRoomsTogether());
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            GenerateCorridors();
        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            RenderFloorPrototype();
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            foreach (DungeonRoom room in _rooms)
            {
                room.Position = Vector2Int.RoundToInt(room.SceneRoom.transform.position);
            }
            _tilesController.GetRoomsToMainGrid(_rooms);
        }

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            _tilesController.DrawWalls(_wallTile);
        }
        */
        

        if (_showConnections)
        {
            if (_corridors != null)
            {
                foreach (Corridor corridor in _corridors)
                {
                    for (int i = 0; i < corridor.Positions.Count - 1; i++)
                    {
                        Debug.DrawLine(new Vector3(corridor.Positions[i].x, corridor.Positions[i].y), new Vector3(corridor.Positions[i + 1].x, corridor.Positions[i + 1].y));
                    }
                }
            }
        }
    }
    #endregion

    #region Floor Logic
    private IEnumerator GenerateLevel()
    {
        GenerateFloor();
        RenderFloor();
        yield return StartCoroutine(PullRoomsTogether());
        GetRoomsToGrid();
        GenerateCorridors();
        _tilesController.DrawWalls(_wallTile);
        InstantiatePrefabs();
        _tilesController.CleanWalls();
        _tilesController.SetMinimap();

        yield return new WaitForSeconds(0.5f);

        AstarPath.active.Scan();
        SpawnPlayer();

        AudioManager.Instance.SetMusicVolume(0.7f);
        AudioManager.Instance.PlayMusic("Dungeon");

        if (LoadingScreen.Instance != null)
        {
            LoadingScreen.Instance.gameObject.SetActive(false);
            UIController.Instance.GetComponent<CanvasGroup>().alpha = 1;
        }
    }

    /// <summary>
    /// Generates the rooms distribution within the floor
    /// </summary>
    private void GenerateFloor()
    {
        _floorGrid = new DungeonRoom[_gridHeight, _gridWidth];
        _rooms = new List<DungeonRoom>();
        _deadEnds = new List<DungeonRoom>();

        _walker = new Walker(_startPosition, _walkersTimeToLive);

        DungeonRoom startRoom = new DungeonRoom(DungeonRoom.DungeonRoomType.Start, _walker.Position);
        _floorGrid[_walker.Position.x, _walker.Position.y] = startRoom;
        _rooms.Add(startRoom);

        int roomsLeft = _maxRooms;

        while (roomsLeft > 0)
        {
            //Debug.Log("Walker position: " + walker.Position + " and rooms left: " + roomsLeft);

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

        foreach (DungeonRoom room in _rooms)
        {
            // Adds all the rooms that are in a extreme of the map
            if (room.ConnectedRooms.Count < 2 && room.Type != DungeonRoom.DungeonRoomType.Start)
            {
                _deadEnds.Add(room);
            }

            // Sets the depth of each generated room
            foreach (DungeonRoom connectedRoom in room.ConnectedRooms)
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
        // Checks for loops
        foreach (DungeonRoom room in _rooms)
        {
            if (room.ConnectedRooms.Count > 1)
            {
                int shallowerRooms = 0;

                foreach (DungeonRoom connectedRoom in room.ConnectedRooms)
                {
                    if (connectedRoom.Depth < room.Depth) shallowerRooms++;
                }

                if (shallowerRooms >= 2)
                {
                    room.Type = DungeonRoom.DungeonRoomType.Loop;
                }
            }
        }
        
        // Creates the boss room as far from the start as possible
        CreateBossRoom();
        DungeonRoom bossRoom = _deadEnds[^1];
        bossRoom.Type = DungeonRoom.DungeonRoomType.Boss;

        // Creates the character room
        foreach (DungeonRoom deadEnd in _deadEnds)
        {
            if (deadEnd.Type == DungeonRoom.DungeonRoomType.Normal)
            {
                deadEnd.Type = DungeonRoom.DungeonRoomType.Weapon;
                _weaponRoom = deadEnd;
                break;
            }
        }

        // Creates the treasure room
        foreach (DungeonRoom deadEnd in _deadEnds)
        {
            if (deadEnd.Type == DungeonRoom.DungeonRoomType.Normal)
            {
                deadEnd.Type = DungeonRoom.DungeonRoomType.Treasure;
                _treasureRoom = deadEnd;
                break;
            }
        }
    }

    /// <summary>
    /// Creates a new Room and its connections
    /// </summary>
    private void CreateRoom()
    {
        DungeonRoom newRoom = new DungeonRoom(DungeonRoom.DungeonRoomType.Normal, _walker.Position);
        DungeonRoom previousRoom = _floorGrid[_walker.PreviousPosition.x, _walker.PreviousPosition.y];

        newRoom.AddConnectedRoom(previousRoom);
        previousRoom.AddConnectedRoom(newRoom);

        _floorGrid[_walker.Position.x, _walker.Position.y] = newRoom;
        _rooms.Add(newRoom);
    }

    /// <summary>
    /// Creates the boss room 
    /// </summary>
    private void CreateBossRoom()
    {
        DungeonRoom checkpointRoom = _deadEnds[^1];
        checkpointRoom.Type = DungeonRoom.DungeonRoomType.Checkpoint;

        _walker.PreviousPosition = Vector2Int.RoundToInt(checkpointRoom.Position);
        _walker.Position = Vector2Int.RoundToInt(checkpointRoom.Position + (checkpointRoom.Position - checkpointRoom.ConnectedRooms[0].Position));

        CreateRoom();
        _deadEnds.RemoveAt(_deadEnds.Count - 1);
        _deadEnds.Add(_rooms[^1]);
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
        else
        {
            if (Random.Range(0f, 1f) < _connectionChance)
            {
                DungeonRoom currentRoom = _floorGrid[_walker.Position.x, _walker.Position.y];
                DungeonRoom previousRoom = _floorGrid[_walker.PreviousPosition.x, _walker.PreviousPosition.y];

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
    private void RenderFloorPrototype()
    {
        foreach (DungeonRoom room in _rooms)
        {
            room.Position *= _movementScalar;

            // Center to 0
            room.Position -= _startPosition * _movementScalar;
            GameObject newRoom;

            switch (room.Type)
            {
                case DungeonRoom.DungeonRoomType.Start:
                    newRoom = Instantiate(_testingRoom, room.Position, Quaternion.identity);
                    newRoom.GetComponent<SpriteRenderer>().color = new Color(1, 1, 0);
                    room.AddSceneRoom(newRoom);
                    break;

                case DungeonRoom.DungeonRoomType.Normal:
                    newRoom = Instantiate(_testingRoom, room.Position, Quaternion.identity);

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

                case DungeonRoom.DungeonRoomType.Treasure:
                    newRoom = Instantiate(_testingRoom, room.Position, Quaternion.identity);
                    newRoom.GetComponent<SpriteRenderer>().color = new Color(1, 200f/255, 0);
                    room.AddSceneRoom(newRoom);
                    break;

                case DungeonRoom.DungeonRoomType.Weapon:
                    newRoom = Instantiate(_testingRoom, room.Position, Quaternion.identity);
                    newRoom.GetComponent<SpriteRenderer>().color = new Color(0, 0, 1);
                    room.AddSceneRoom(newRoom);
                    break;

                case DungeonRoom.DungeonRoomType.Loop:
                    newRoom = Instantiate(_testingRoom, room.Position, Quaternion.identity);
                    newRoom.GetComponent<SpriteRenderer>().color = new Color(0, 1, 0);
                    room.AddSceneRoom(newRoom);
                    break;

                case DungeonRoom.DungeonRoomType.Checkpoint:
                    newRoom = Instantiate(_testingRoom, room.Position, Quaternion.identity);
                    newRoom.GetComponent<SpriteRenderer>().color = new Color(1, 0, 1);
                    room.AddSceneRoom(newRoom);
                    break;

                case DungeonRoom.DungeonRoomType.Boss:
                    newRoom = Instantiate(_testingRoom, room.Position, Quaternion.identity);
                    newRoom.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0);
                    room.AddSceneRoom(newRoom);
                    break;
            }
        }
    }

    /// <summary>
    /// Renders the floor with the selected rooms once the loop has ended
    /// </summary>
    private void RenderFloor()
    {
        foreach(DungeonRoom room in _rooms)
        {
            room.Position *= _movementScalar;

            // Center to 0
            room.Position -= _startPosition * _movementScalar;
            GameObject newRoom;
            DungeonRoomData roomData = null;

            switch (room.Type)
            {
                case DungeonRoom.DungeonRoomType.Start:
                    roomData = _startRoom;
                    newRoom = Instantiate(roomData.roomTilemap, room.Position, Quaternion.identity);
                    room.AddSceneRoom(newRoom);
                    break;

                case DungeonRoom.DungeonRoomType.Normal:
                    roomData = _normalRooms[Random.Range(0, _normalRooms.Length)];
                    newRoom = Instantiate(roomData.roomTilemap, room.Position, Quaternion.identity);
                    room.AddSceneRoom(newRoom);
                    break;

                case DungeonRoom.DungeonRoomType.Treasure:
                    roomData = _treasureRoomData;
                    newRoom = Instantiate(roomData.roomTilemap, room.Position, Quaternion.identity);
                    room.AddSceneRoom(newRoom);
                    break;

                case DungeonRoom.DungeonRoomType.Weapon:
                    roomData = _weaponRoomData;
                    newRoom = Instantiate(roomData.roomTilemap, room.Position, Quaternion.identity);
                    room.AddSceneRoom(newRoom);
                    break;

                case DungeonRoom.DungeonRoomType.Loop:
                    roomData = _normalRooms[Random.Range(0, _normalRooms.Length)];
                    newRoom = Instantiate(roomData.roomTilemap, room.Position, Quaternion.identity);
                    room.AddSceneRoom(newRoom);
                    break;

                case DungeonRoom.DungeonRoomType.Checkpoint:
                    roomData = _managementRoom;
                    newRoom = Instantiate(roomData.roomTilemap, room.Position, Quaternion.identity);
                    room.AddSceneRoom(newRoom);
                    break;

                case DungeonRoom.DungeonRoomType.Boss:
                    roomData = _bossRoom;
                    newRoom = Instantiate(roomData.roomTilemap, room.Position, Quaternion.identity);
                    room.AddSceneRoom(newRoom);
                    break;
            }

            // Stores the room enemies to the room data
            if (roomData != null)
            {
                room.EnemyPositions = roomData.enemyPositions.ToList();
                room.EnemyPool = roomData.enemyPool.ToList();
                room.EnemyTypeLimits = roomData.enemyTypeLimits.ToList();
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
        for (int i = 0; i < _rooms.Count; i++)
        {
            if (_rooms[i].SceneRoom.TryGetComponent<Rigidbody2D>(out var rigidbody))
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
                if (Random.Range(0f, 1f) < 0.8f)
                {
                    velocity = Vector2.MoveTowards(sceneRooms[i].position, _rooms[0].Position, _pullSpeed);
                }
                else
                {
                    velocity = Vector2.MoveTowards(sceneRooms[i].position, new Vector2(sceneRooms[i].position.x + Random.Range(-1, 1), sceneRooms[i].position.y + Random.Range(-1, 1)), _pullSpeed);
                }

                sceneRooms[i].velocity = -velocity;
            }
        }

        // Waits and stops the rooms from moving again
        yield return new WaitForSeconds(0.1f);

        foreach (DungeonRoom room in _rooms)
        {
            if (room.SceneRoom.TryGetComponent<Rigidbody2D>(out var rigidbody))
            {
                rigidbody.velocity = Vector2.zero;
            }
            room.SceneRoom.transform.position = Vector3Int.RoundToInt(room.SceneRoom.transform.position);
        }
    }

    private void GetRoomsToGrid()
    {
        foreach (DungeonRoom room in _rooms)
        {
            if (room.Type == DungeonRoom.DungeonRoomType.Start) _startPosition = Vector2Int.RoundToInt(room.SceneRoom.transform.position);
            room.Position = Vector2Int.RoundToInt(room.SceneRoom.transform.position);
        }
        _tilesController.GetRoomsToMainGrid(_rooms);
    }
    #endregion

    #region Corridors
    /// <summary>
    /// Generates the corridors that connect the existing rooms
    /// </summary>
    private void GenerateCorridors()
    {
        _corridors = new List<Corridor>();

        foreach (DungeonRoom room in _rooms)
        {
            foreach (DungeonRoom connectedRoom in room.ConnectedRooms)
            {
                if (!CheckDuplicatedCorridors(room, connectedRoom))
                {
                    Vector2Int connectedRoomPosition = Vector2Int.RoundToInt(connectedRoom.Position);
                    Corridor corridor = new Corridor(room, connectedRoom);

                    bool moveHorizontal = CheckConnectedRoomPosition(corridor.Positions[^1], connectedRoomPosition);
                    SetCorridorLine(moveHorizontal, connectedRoomPosition, corridor);  
                    
                    room.AddCorridor(corridor);
                    connectedRoom.AddCorridor(corridor);
                    _corridors.Add(corridor);
                }
            }
        }

        _tilesController.DrawCorridors(_corridors);
    }

    /// <summary>
    /// Sets the positions of the corridor while it is straight
    /// </summary>
    /// <param name="horizontal"></param>
    /// <param name="connectedRoomPosition">Position of the room to connect</param>
    /// <param name="corridor">Current corridor</param>
    private void SetCorridorLine(bool horizontal, Vector2Int connectedRoomPosition, Corridor corridor)
    {
        Vector2Int originalPosition = corridor.Positions[0];
        Vector2Int position = originalPosition;

        if (horizontal)
        {
            SetCorridorHorizontalLine(ref position, connectedRoomPosition, corridor);

            if (CorridorCollides(corridor.Positions[^1], corridor.DestinationRoom, corridor.OriginRoom))
            {
                corridor.ResetPositions();
                position = originalPosition;

                SetCorridorVerticalLine(ref position, connectedRoomPosition, corridor);
                SetCorridorHorizontalLine(ref position, connectedRoomPosition, corridor);
            }
            else
            {
                SetCorridorVerticalLine(ref position, connectedRoomPosition, corridor);
            }
        }

        else
        {
            SetCorridorVerticalLine(ref position, connectedRoomPosition, corridor);

            if (CorridorCollides(corridor.Positions[^1], corridor.DestinationRoom, corridor.OriginRoom))
            {
                corridor.ResetPositions();
                position = originalPosition;

                SetCorridorHorizontalLine(ref position, connectedRoomPosition, corridor);
                SetCorridorVerticalLine(ref position, connectedRoomPosition, corridor);
            }
            else
            {
                SetCorridorHorizontalLine(ref position, connectedRoomPosition, corridor);
            }
        }
    }

    private void SetCorridorHorizontalLine(ref Vector2Int position, Vector2Int connectedRoomPosition, Corridor corridor)
    {
        while (position.x != connectedRoomPosition.x)
        {
            if (position.x < connectedRoomPosition.x) position.x += 1;
            else position.x -= 1;
            corridor.AddNewPosition(position, true);

            if (IsWithinRoomBounds(position, corridor.DestinationRoom) && _tilesController.FloorTilemap.HasTile((Vector3Int)position)) break;
        }
    }

    private void SetCorridorVerticalLine(ref Vector2Int position, Vector2Int connectedRoomPosition, Corridor corridor)
    {
        while (position.y != connectedRoomPosition.y)
        {
            if (position.y < connectedRoomPosition.y) position.y += 1;
            else position.y -= 1;
            corridor.AddNewPosition(position, false);

            if (IsWithinRoomBounds(position, corridor.DestinationRoom) && _tilesController.FloorTilemap.HasTile((Vector3Int)position)) break;
        }
    }

    private bool IsWithinRoomBounds(Vector2Int position, DungeonRoom room)
    {
        if (position.x < room.Position.x + (room.Width / 2) && position.x > room.Position.x - (room.Width / 2) &&
            position.y < room.Position.y + (room.Height / 2) && position.y > room.Position.y - (room.Height / 2))
        {
            return true;
        }

        else
        {
            return false;
        }
    }
    
    /// <summary>
    /// Checks if the positions is within any existing room
    /// </summary>
    /// <param name="position">Position to check</param>
    /// <param name="destinationRoom">Destination room</param>
    /// <param name="originRoom">Origin room</param>
    /// <returns></returns>
    private bool CorridorCollides(Vector2Int position, DungeonRoom destinationRoom, DungeonRoom originRoom)
    {
        foreach(DungeonRoom room in _rooms)
        {
            if (room == destinationRoom || room == originRoom) continue;
            
            if (position.x < room.Position.x + (room.Width / 2) && position.x > room.Position.x - (room.Width / 2) &&
                position.y < room.Position.y + (room.Height / 2) && position.y > room.Position.y - (room.Height / 2))
            {
                Debug.Log("Collision at: " + position + " Origin: " + originRoom.Position + " Destiny: " + destinationRoom.Position + " with room: + " + room.Position);
                return true;
            }
            
        }

        return false;
    }
    

    /// <summary>
    /// Checks for the corridors to avoid creating duplicated ones
    /// </summary>
    /// <returns>If the corridor already exists</returns>
    private bool CheckDuplicatedCorridors(DungeonRoom room, DungeonRoom connectedRoom)
    {
        // Avoids repeated corridors
        foreach (DungeonRoom otherRoom in _rooms)
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

    /// <summary>
    /// Checks the position of the room to connect
    /// </summary>
    /// <param name="roomPosition">Origin room position</param>
    /// <param name="connectedRoomPosition">Destination room position</param>
    /// <returns>If the room is closer in the x or the y axis</returns>
    private bool CheckConnectedRoomPosition(Vector2 roomPosition, Vector2 connectedRoomPosition)
    {
        Vector2 positionDif = roomPosition - connectedRoomPosition;

        if (Mathf.Abs(positionDif.x) > Mathf.Abs(positionDif.y)) return true;
        else return false;
    }
    #endregion

    #region Items & Weapons
    private void InstantiatePrefabs()
    {
        Vector3 offset = new Vector2(0, .5f);

        // Item spawn
        Vector3 itemPos = _treasureRoom.Position;
        ScenePassiveItem item = Instantiate(_passiveItemPrefab, itemPos - offset, Quaternion.identity).GetComponentInChildren<ScenePassiveItem>();
        item.SetBaseItem(_itemsPool[Random.Range(0, _itemsPool.Length)]);

        // Weapon spawn
        Vector3 weaponPos = _weaponRoom.Position;
        SceneWeapon weapon = Instantiate(_weaponPrefab, weaponPos - offset, Quaternion.identity).GetComponentInChildren<SceneWeapon>();
        weapon.SetBaseWeapon(_weaponsPool[Random.Range(0, _weaponsPool.Length)]);

        // Enemies spawn
        foreach (DungeonRoom room in _rooms)
        {
            foreach (Vector2 position in room.EnemyPositions)
            {
                int enemyIndex;

                do
                {
                    enemyIndex = Random.Range(0, room.EnemyPool.Count);
                }
                while (room.EnemyTypeLimits[enemyIndex] == 0);

                room.EnemyTypeLimits[enemyIndex] -= 1;
                GameObject enemy = room.EnemyPool[enemyIndex];

                Instantiate(enemy, room.Position + position, Quaternion.identity);
            }
        }

        List<GameObject> prefabs = new List<GameObject>();

        offset.x = 0.5f;
        offset.y = 0.5f;

        // Decoration
        foreach (Vector3Int position in _tilesController.FloorTilemap.cellBounds.allPositionsWithin)
        {
            // Floor decoration
            if (HasClearSurroundings(position, prefabs) && Random.value < _prefabChance)
            {
                prefabs.Add(Instantiate(_decoration[Random.Range(0, _decoration.Length)], position + offset, Quaternion.identity));
            }

            // Walls decoration
            if (_tilesController.WallsTilemap.HasTile(position + Vector3Int.up) && _tilesController.FloorTilemap.HasTile(position) 
                && !_tilesController.WallsTilemap.HasTile(position) && Random.value < _wallPrefabChance)
            {
                GameObject wallProp = Instantiate(_wallDecoration[Random.Range(0, _wallDecoration.Length)], position + offset, Quaternion.identity);
                _tilesController.SimplePrefabToMainGrid(wallProp, _tilesController.DetailsTilemap);
            }
        }
    }

    private bool HasClearSurroundings(Vector3Int position, List<GameObject> prefabs)
    {
        int tileRange = 1;
        int minDistance = 5;

        // Checks if it has floor tiles around
        for (int x = position.x - tileRange; x <= position.x + tileRange; x++)
        {
            for (int y = position.y - tileRange; y <= position.y + tileRange; y++)
            {
                if (!_tilesController.FloorTilemap.HasTile(new Vector3Int(x, y)) || _tilesController.HolesTilemap.HasTile(new Vector3Int(x, y)) || 
                    _tilesController.ObstaclesTilemap.HasTile(new Vector3Int(x, y)) || _tilesController.WallsTilemap.HasTile(new Vector3Int(x, y)))
                {
                    //Debug.Log("Has no tile");
                    return false;
                }
            }
        }        

        foreach(GameObject prefab in prefabs)
        {
            if (Vector3.Distance(prefab.transform.position, position) < minDistance)
            {
                return false;
            }
        }

        return true;
    }
    #endregion

    private void SpawnPlayer()
    {
        _player.transform.position = (Vector3Int)_startPosition;
        _player.GetComponent<PlayerController>().EnableControls();

        Vector3 cameraPos = CameraController.Instance.transform.position;
        cameraPos.x = _player.transform.position.x;
        cameraPos.y = _player.transform.position.y;
        CameraController.Instance.transform.position = cameraPos;
    }
}