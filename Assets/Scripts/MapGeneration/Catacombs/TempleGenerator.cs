using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class TempleGenerator : MonoBehaviour
{
    #region Global Variables
    [SerializeField] private TilesController _tilesController;
    [SerializeField] private GameObject _player;

    [Header("Rooms")]
    [SerializeField] private GameObject _testingRoom;
    [SerializeField] private GameObject _testingRoomLongH;
    [SerializeField] private GameObject _testingRoomLongV;
    [SerializeField] private GameObject _testingRoomBig;
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
    [SerializeField] private int _minEmptyAdjacentRooms;
    [Range(0, 4)]
    [SerializeField] private int _minEmptyAdjacentRoomsLongH;
    [Range(0, 4)]
    [SerializeField] private int _minEmptyAdjacentRoomsLongV;
    [Range(0, 8)]
    [SerializeField] private int _minEmptyAdjacentRoomsBig;
    [SerializeField] private int _minDeadEnds;
    [SerializeField] private int _maxDeadEnds;
    [SerializeField] private int _minDepth;
    [Range(0, 1)]
    [SerializeField] private float _longRoomChance;
    [Range(0, 1)]
    [SerializeField] private float _bigRoomChance;
    [SerializeField] private int _maxLongRooms;
    [SerializeField] private int _maxBigRooms;

    [Header("Walkers Params")]
    [SerializeField] private int _walkersTimeToLive;

    [Header("Generation Params")]
    [SerializeField] private TileBase _wallTile;

    private Vector2Int _startPosition;
    private Walker _walker;
    private TempleRoom[,] _floorGrid;
    private List<TempleRoom> _rooms;
    private List<TempleRoom> _deadEnds;

    private int _iterations;
    private int _longRoomsCount;
    private int _bigRoomsCount;
    #endregion

    #region Unity Methods
    void Awake()
    {
        _startPosition = new Vector2Int(_gridHeight / 2, _gridWidth / 2);
    }

    private void Start()
    {
        GenerateFloor();
        RenderFloorPrototype();
        LoadingScreen.Instance.gameObject.SetActive(false);
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
    #endregion

    #region Floor Generation
    /// <summary>
    /// Generates the rooms distribution within the floor   
    /// </summary>
    private void GenerateFloor()
    {
        Debug.Log("START");
        _floorGrid = new TempleRoom[_gridHeight, _gridWidth];
        _rooms = new List<TempleRoom>();
        _deadEnds = new List<TempleRoom>();
        _longRoomsCount = 0;
        _bigRoomsCount = 0;

        _walker = new Walker(_startPosition, _walkersTimeToLive);

        TempleRoom startRoom = new TempleRoom(TempleRoom.TempleRoomType.Start, _walker.Position);
        _floorGrid[_walker.Position.x, _walker.Position.y] = startRoom;
        _rooms.Add(startRoom);

        int roomsLeft = _maxRooms;

        while (roomsLeft > 0)
        {
            //Debug.Log("Walker position: " + _walker.Position + " and rooms left: " + roomsLeft);

            if (_walker.TimeToLive <= 0)
            {
                // Resets the walker to the start (no need to create a new one)
                _walker.Position = _startPosition;
                _walker.TimeToLive = _walkersTimeToLive;
            }

            MoveWalker();

            if (WalkerLocationIsValid(out var type))
            {
                CreateRoom(type);
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

        foreach (TempleRoom room in _rooms)
        {
            // Adds all the rooms that are in a extreme of the map
            if (room.ConnectedRooms.Count < 2 && room.Type == TempleRoom.TempleRoomType.Normal)
            {
                _deadEnds.Add(room);
            }

            // Sets the depth of each generated room
            foreach (TempleRoom connectedRoom in room.ConnectedRooms)
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
        TempleRoom bossRoom = _deadEnds[^1];
        bossRoom.Type = TempleRoom.TempleRoomType.Boss;

        // Sets the key room as far from the boss room as possible
        TempleRoom keyRoom = bossRoom;
        float prevDistance = 0;

        foreach (TempleRoom deadEnd in _deadEnds)
        {
            if (deadEnd.Type == TempleRoom.TempleRoomType.Normal)
            {
                float currentDistance = Vector2Int.Distance(deadEnd.Position, bossRoom.Position);
                if (currentDistance > prevDistance)
                {
                    prevDistance = currentDistance;
                    keyRoom = deadEnd;
                }
            }
        }
        keyRoom.Type = TempleRoom.TempleRoomType.KeyRoom;

        // Creates the character room
        foreach (TempleRoom deadEnd in _deadEnds)
        {
            if (deadEnd.Type == TempleRoom.TempleRoomType.Normal)
            {
                deadEnd.Type = TempleRoom.TempleRoomType.Character;
                break;
            }
        }

        // Creates the treasure room
        foreach (TempleRoom deadEnd in _deadEnds)
        {
            if (deadEnd.Type == TempleRoom.TempleRoomType.Normal)
            {
                deadEnd.Type = TempleRoom.TempleRoomType.Treasure;
                break;
            }
        }
    }
    #endregion

    #region Room Creation
    /// <summary>
    /// Creates a new Room and its connections
    /// </summary>
    private void CreateRoom(TempleRoom.TempleRoomType type)
    {
        TempleRoom newRoom = new TempleRoom(type, _walker.Position);
        TempleRoom previousRoom = _floorGrid[_walker.PreviousPosition.x, _walker.PreviousPosition.y];

        newRoom.AddConnectedRoom(previousRoom);
        previousRoom.AddConnectedRoom(newRoom);

        _floorGrid[_walker.Position.x, _walker.Position.y] = newRoom;
        newRoom.OccupiedGridPositions.Add(_walker.Position);

        Vector2Int offset = Vector2Int.zero;

        switch (type)
        {
            case TempleRoom.TempleRoomType.LongHorizontal:

                if (_walker.Direction.x != 0)
                {
                    offset = _walker.Direction;
                }
                else
                {
                    // If the walker comes from up or down, the long room offset will be to the right
                    offset = Vector2Int.right;
                }

                _floorGrid[_walker.Position.x + offset.x, _walker.Position.y] = newRoom;

                newRoom.OccupiedGridPositions.Add(_walker.Position + offset);
                Vector2Int[] orderedPositions = newRoom.OccupiedGridPositions.OrderByDescending(position => position.x).ToArray<Vector2Int>();
                newRoom.Position = orderedPositions[^1];

                _longRoomsCount++;
                break;

            case TempleRoom.TempleRoomType.LongVertical:

                if (_walker.Direction.y != 0)
                {
                    offset = _walker.Direction;
                }
                else
                {
                    // If the walker comes from up or down, the long room offset will be to the right
                    offset = Vector2Int.up;
                }

                _floorGrid[_walker.Position.x, _walker.Position.y + offset.y] = newRoom;

                newRoom.OccupiedGridPositions.Add(_walker.Position + offset);
                orderedPositions = newRoom.OccupiedGridPositions.OrderByDescending(position => position.y).ToArray<Vector2Int>();
                newRoom.Position = orderedPositions[^1];

                _longRoomsCount++;
                break;

            case TempleRoom.TempleRoomType.Big:

                offset = _walker.Direction;
                if (_walker.Direction.x == 0) offset.x = 1;
                if (_walker.Direction.y == 0) offset.y = 1;

                _floorGrid[_walker.Position.x + offset.x, _walker.Position.y] = newRoom;
                _floorGrid[_walker.Position.x, _walker.Position.y + offset.y] = newRoom;
                _floorGrid[_walker.Position.x + offset.x, _walker.Position.y + offset.y] = newRoom;

                newRoom.OccupiedGridPositions.Add(new Vector2Int(_walker.Position.x + offset.x, _walker.Position.y));
                newRoom.OccupiedGridPositions.Add(new Vector2Int(_walker.Position.x, _walker.Position.y + offset.y));
                newRoom.OccupiedGridPositions.Add(_walker.Position + offset);

                orderedPositions = newRoom.OccupiedGridPositions.OrderByDescending(position => position.magnitude).ToArray<Vector2Int>();
                newRoom.Position = orderedPositions[^1];

                _bigRoomsCount++;
                break;
        }

        //Debug.Log("Room of type: " + newRoom.Type.HumanName() + " created in Position: " + newRoom.Position + ". Occupied positions:");
        foreach(Vector2Int pos in newRoom.OccupiedGridPositions)
        {
            Debug.Log(pos);
        }
        _rooms.Add(newRoom);
    }

    private bool NormalRoomFits()
    {
        int emptyRooms = 0;

        if (_floorGrid[_walker.Position.x + 1, _walker.Position.y] == null) emptyRooms++;
        if (_floorGrid[_walker.Position.x - 1, _walker.Position.y] == null) emptyRooms++;
        if (_floorGrid[_walker.Position.x, _walker.Position.y + 1] == null) emptyRooms++;
        if (_floorGrid[_walker.Position.x, _walker.Position.y - 1] == null) emptyRooms++;

        if (emptyRooms > _minEmptyAdjacentRooms)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Checks if a Lon room in horizontal fits. Es podria fer creant primer la sala i iterant per les seves posicions, així es podrien fer sales de moltes formes diferents i s'adaptaria bé
    /// </summary>
    /// <returns></returns>
    private bool LongRoomHorizontalFits()
    {
        int offset;

        if (_walker.Direction.x != 0)
        {
            offset = _walker.Direction.x;
        }
        else
        {

            offset = 1;
        }

        int emptyRooms = 0;

        if (_floorGrid[_walker.Position.x + offset, _walker.Position.y] != null) return false;

        // Check for the neighbours
        if (_floorGrid[_walker.Position.x, _walker.Position.y + 1] == null) emptyRooms++;
        if (_floorGrid[_walker.Position.x, _walker.Position.y - 1] == null) emptyRooms++;
        if (_floorGrid[_walker.Position.x + offset, _walker.Position.y + 1] == null) emptyRooms++;
        if (_floorGrid[_walker.Position.x + offset, _walker.Position.y - 1] == null) emptyRooms++;
        if (_floorGrid[_walker.Position.x + (offset * 2), _walker.Position.y] == null) emptyRooms++;
        if (_floorGrid[_walker.Position.x - offset, _walker.Position.y] == null) emptyRooms++;

        if (emptyRooms > _minEmptyAdjacentRoomsLongH)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool LongRoomVerticalFits()
    {
        int offset;

        if (_walker.Direction.y != 0)
        {
            offset = _walker.Direction.y;
        }
        else
        {

            offset = 1;
        }

        int emptyRooms = 0;

        if (_floorGrid[_walker.Position.x, _walker.Position.y + offset] != null) return false;

        // Check for the neighbours
        if (_floorGrid[_walker.Position.x + 1, _walker.Position.y] == null) emptyRooms++;
        if (_floorGrid[_walker.Position.x - 1, _walker.Position.y] == null) emptyRooms++;
        if (_floorGrid[_walker.Position.x + 1, _walker.Position.y + offset] == null) emptyRooms++;
        if (_floorGrid[_walker.Position.x - 1, _walker.Position.y + offset] == null) emptyRooms++;
        if (_floorGrid[_walker.Position.x, _walker.Position.y + (offset * 2)] == null) emptyRooms++;
        if (_floorGrid[_walker.Position.x, _walker.Position.y - offset] == null) emptyRooms++;

        if (emptyRooms > _minEmptyAdjacentRoomsLongV)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool BigRoomFits()
    {
        Vector2Int offset = _walker.Direction;

        if (_walker.Direction.x == 0) offset.x = 1;
        if (_walker.Direction.y == 0) offset.y = 1;

        int emptyRooms = 0;

        if (_floorGrid[_walker.Position.x, _walker.Position.y + offset.y] != null ||
            _floorGrid[_walker.Position.x + offset.x, _walker.Position.y] != null ||
            _floorGrid[_walker.Position.x + offset.x, _walker.Position.y + offset.y] != null) return false;

        // Check for the neighbours
        if (_floorGrid[_walker.Position.x - offset.x, _walker.Position.y] == null) emptyRooms++;
        if (_floorGrid[_walker.Position.x - offset.x, _walker.Position.y + offset.y] == null) emptyRooms++;
        if (_floorGrid[_walker.Position.x, _walker.Position.y + (offset.y * 2)] == null) emptyRooms++;
        if (_floorGrid[_walker.Position.x + offset.x, _walker.Position.y + (offset.y * 2)] == null) emptyRooms++;
        if (_floorGrid[_walker.Position.x + (offset.x * 2), _walker.Position.y + offset.y] == null) emptyRooms++;
        if (_floorGrid[_walker.Position.x + (offset.x * 2), _walker.Position.y] == null) emptyRooms++;
        if (_floorGrid[_walker.Position.x + offset.x, _walker.Position.y - offset.y] == null) emptyRooms++;
        if (_floorGrid[_walker.Position.x, _walker.Position.y - offset.y] == null) emptyRooms++;

        if (emptyRooms > _minEmptyAdjacentRoomsBig)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion

    #region Walker
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
            if (Random.Range(0f, 1f) > 0.5f)
            {
                movement.x = 1;
                _walker.Direction = Vector2Int.right;
            }
            else
            {
                movement.x = -1;
                _walker.Direction = Vector2Int.left;
            }
        }
        else
        {
            // Move vertical
            if (Random.Range(0f, 1f) > 0.5f)
            {
                movement.y = 1;
                _walker.Direction = Vector2Int.up;
            }
            else
            {
                movement.y = -1;
                _walker.Direction = Vector2Int.down;
            }
        }

        _walker.Position += movement;
    }

    /// <summary>
    /// Checks if the current position is valid to create a new Room
    /// </summary>
    private bool WalkerLocationIsValid(out TempleRoom.TempleRoomType roomType)
    {
        roomType = TempleRoom.TempleRoomType.Normal;

        if (_floorGrid[_walker.Position.x, _walker.Position.y] == null)
        {
            if (_bigRoomsCount < _maxBigRooms && Random.Range(0, 1f) < _bigRoomChance)
            {
                if (BigRoomFits())
                {
                    roomType = TempleRoom.TempleRoomType.Big;
                    return true;
                }
            }

            if (_longRoomsCount < _maxLongRooms && Random.Range(0, 1f) < _longRoomChance)
            {
                if (LongRoomHorizontalFits())
                {
                    roomType = TempleRoom.TempleRoomType.LongHorizontal;
                    return true;
                }
            }

            if (_longRoomsCount < _maxLongRooms && Random.Range(0, 1f) < _longRoomChance)
            {
                if (LongRoomVerticalFits())
                {
                    roomType = TempleRoom.TempleRoomType.LongVertical;
                    return true;
                }
            }

            if (NormalRoomFits())
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
    #endregion 

    /// <summary>
    /// Renders the floor with the selected rooms once the loop has ended
    /// </summary>
    private void RenderFloorPrototype()
    {
        foreach (TempleRoom room in _rooms)
        {
            // Center to 0
            room.Position -= _startPosition;
            GameObject newRoom;

            //room.Position *= 10;

            Vector3 position = new Vector3(room.Position.x, room.Position.y);
            float value = room.Depth * 30;

            switch (room.Type)
            {
                case TempleRoom.TempleRoomType.Start:
                    newRoom = Instantiate(_testingRoom, position, Quaternion.identity);
                    newRoom.transform.Find("Room").GetComponent<SpriteRenderer>().color = new Color(1, 1, 0);
                    room.AddSceneRoom(newRoom);
                    break;

                case TempleRoom.TempleRoomType.Normal:
                    newRoom = Instantiate(_testingRoom, position, Quaternion.identity);
                    newRoom.transform.Find("Room").GetComponent<SpriteRenderer>().color = new Color((255 - value) / 255, (255 - value) / 255, (255 - value) / 255);
                    room.AddSceneRoom(newRoom);
                    break;

                case TempleRoom.TempleRoomType.LongHorizontal:
                    newRoom = Instantiate(_testingRoomLongH, position, Quaternion.identity);
                    newRoom.transform.Find("Room").GetComponent<SpriteRenderer>().color = new Color((255 - value) / 255, (255 - value) / 255, (255 - value) / 255);
                    room.AddSceneRoom(newRoom);
                    break;

                case TempleRoom.TempleRoomType.LongVertical:
                    newRoom = Instantiate(_testingRoomLongV, position, Quaternion.identity);
                    newRoom.transform.Find("Room").GetComponent<SpriteRenderer>().color = new Color((255 - value) / 255, (255 - value) / 255, (255 - value) / 255);
                    room.AddSceneRoom(newRoom);
                    break;

                case TempleRoom.TempleRoomType.Big:
                    newRoom = Instantiate(_testingRoomBig, position, Quaternion.identity);
                    newRoom.transform.Find("Room").GetComponent<SpriteRenderer>().color = new Color((255 - value) / 255, (255 - value) / 255, (255 - value) / 255);
                    room.AddSceneRoom(newRoom);
                    break;

                case TempleRoom.TempleRoomType.Treasure:
                    newRoom = Instantiate(_testingRoom, position, Quaternion.identity);
                    newRoom.transform.Find("Room").GetComponent<SpriteRenderer>().color = new Color(1, 200f / 255, 0);
                    room.AddSceneRoom(newRoom);
                    break;

                case TempleRoom.TempleRoomType.Character:
                    newRoom = Instantiate(_testingRoom, position, Quaternion.identity);
                    newRoom.transform.Find("Room").GetComponent<SpriteRenderer>().color = new Color(0, 0, 1);
                    room.AddSceneRoom(newRoom);
                    break;

                case TempleRoom.TempleRoomType.KeyRoom:
                    newRoom = Instantiate(_testingRoom, position, Quaternion.identity);
                    newRoom.transform.Find("Room").GetComponent<SpriteRenderer>().color = new Color(0, 1, 1);
                    room.AddSceneRoom(newRoom);
                    break;

                case TempleRoom.TempleRoomType.Boss:
                    newRoom = Instantiate(_testingRoom, position, Quaternion.identity);
                    newRoom.transform.Find("Room").GetComponent<SpriteRenderer>().color = new Color(1, 0, 0);
                    room.AddSceneRoom(newRoom);
                    break;
            }
        }
    }
}
