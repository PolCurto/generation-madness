using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorGenerator : MonoBehaviour
{
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

    public bool stop;

    void Start()
    {
        _floorGrid = new Room[_gridHeight, _gridWidth];
        _roomsList = new List<Room>();
        _startPosition = new Vector2Int(_gridHeight / 2, _gridWidth / 2);
        stop = false;

        GenerateFloor();
        RenderFloor();
        StartCoroutine(PullRoomsTogether());
    }

    /// <summary>
    /// Generates the rooms distribution within the floor
    /// </summary>
    private void GenerateFloor()
    {
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

        ClassifyRooms();
    }

    private void ClassifyRooms()
    {
        _roomsList[0].DepthIsSet = true;

        foreach (Room room in _roomsList)
        {
            foreach (Room connectedRoom in room.ConnectedRooms)
            {
                if (!connectedRoom.DepthIsSet)
                {
                    connectedRoom.Depth = room.Depth + 1;
                    connectedRoom.DepthIsSet = true;
                }
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
            
            Room currentRoom = _floorGrid[walker.Position.x, walker.Position.y];
            Room previousRoom = _floorGrid[walker.PreviousPosition.x, walker.PreviousPosition.y];

            currentRoom.AddConnectedRoom(previousRoom);
            previousRoom.AddConnectedRoom(currentRoom);
            
        }

        return false;
    }

    /// <summary>
    /// Renders the floor with the selected rooms once the loop has ended
    /// </summary>
    private void RenderFloor()
    {
        foreach(Room room in _roomsList)
        {
            room.Positon *= _movementScalar;

            // Center to 0
            room.Positon -= _startPosition * _movementScalar;
            GameObject newRoom;

            switch (room.Type)
            {
                case Room.RoomType.Start:
                    newRoom = Instantiate(_startRoom, room.Positon, Quaternion.identity);
                    room.AddSceneRoom(newRoom);
                    break;

                case Room.RoomType.Normal:
                    newRoom = Instantiate(_normalRooms[0], room.Positon, Quaternion.identity);

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

                case Room.RoomType.Boss:
                    break;
            }
        }
    }

    private IEnumerator PullRoomsTogether()
    {
        float timer = 0;
        Vector2 velocity = Vector2.zero;

        Vector2[] directions = new Vector2[_roomsList.Count];
        List<Rigidbody2D> sceneRooms = new List<Rigidbody2D>();

        for (int i = 0; i < _roomsList.Count; i++)
        {
            if (_roomsList[i]._sceneRoom.TryGetComponent<Rigidbody2D>(out var rigidbody))
            {
                sceneRooms.Add(rigidbody);
            }
        }

        while (timer < _pullTime)
        {
            yield return null;

            timer += Time.deltaTime;

            for (int i = 0; i < sceneRooms.Count; i++)
            {
                //directions[i] = (_roomsList[0].Positon - sceneRooms[i].position).normalized;
                velocity = Vector2.MoveTowards(sceneRooms[i].position, _roomsList[0].Positon, _pullSpeed);
                //velocity.y = Mathf.MoveTowards(velocity.y, .005f * directions[i].y, .1f * Time.fixedDeltaTime);

                sceneRooms[i].position = velocity;
            }
        }

    }

    private void Update()
    {
        foreach (Room room in _roomsList)
        {
            if (stop)
            {
                if (room._sceneRoom.TryGetComponent<Rigidbody2D>(out var rigidbody))
                {
                    rigidbody.velocity = Vector2.zero;

                }
            }

            foreach (Room connectedRoom in room.ConnectedRooms)
            {
                Debug.DrawLine(room._sceneRoom.transform.position, connectedRoom._sceneRoom.transform.position);
            }
        }
    }
}