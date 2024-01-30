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

    [Header("Walkers Params")]
    //[SerializeField] private int _numWalkers;
    [SerializeField] private int _walkersTimeToLive;

    private Vector2Int _startPosition;
    private Walker walker;
    private Room[,] _floorGrid;
    private List<Room> _roomsList;
    private List<Rigidbody2D> _sceneRoomsList;

    void Start()
    {
        _floorGrid = new Room[_gridHeight, _gridWidth];
        _roomsList = new List<Room>();
        _sceneRoomsList = new List<Rigidbody2D>();
        _startPosition = new Vector2Int(_gridHeight / 2, _gridWidth / 2);

        GenerateFloor();
        RenderFloor();
        PullRoomsTogether();
    }

    /// <summary>
    /// Generates the rooms distribution within the floor
    /// </summary>
    private void GenerateFloor()
    {
        walker = new Walker(_startPosition, _walkersTimeToLive);

        Room startRoom = new Room(0, Room.RoomType.Start, walker.Position);
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
    }

    /// <summary>
    /// Creates a new Room and its connections
    /// </summary>
    private void CreateRoom()
    {
        Room newRoom = new Room(0, Room.RoomType.Normal, walker.Position);
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

            if (emptyRooms > 2)
            {
                return true;
            }
            else
            {
                walker.Position = walker.PreviousPosition;
            }
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
            room.Positon *= new Vector2(_movementScalar, _movementScalar);

            switch (room.Type)
            {
                case Room.RoomType.Start:
                    Instantiate(_startRoom, room.Positon, Quaternion.identity);
                    break;

                case Room.RoomType.Normal:
                    GameObject newRoom = Instantiate(_normalRooms[0], room.Positon, Quaternion.identity);
                    _sceneRoomsList.Add(newRoom.GetComponent<Rigidbody2D>());
                    break;

                case Room.RoomType.Boss:
                    break;
            }
        }
    }

    private IEnumerator PullRoomsTogether()
    {
        float timer = 0;
        Vector2 velocity = Vector2.one;

        while (timer < 2000)
        {
            timer += Time.deltaTime;

            foreach (Rigidbody2D room in _sceneRoomsList)
            {
                Vector2 direction = room.position - _roomsList[0].Positon;

                velocity.x = Mathf.MoveTowards(velocity.x, 1 * direction.x, 100 * Time.fixedDeltaTime);
                velocity.y = Mathf.MoveTowards(velocity.y, 1 * direction.y, 100 * Time.fixedDeltaTime);

                room.velocity = velocity;
            }
        }

        foreach (Rigidbody2D room in _sceneRoomsList)
        {
            room.velocity = Vector2.zero;
        }
    }


    private void Update()
    {
        foreach (Room room in _roomsList)
        {
            foreach (Room connectedRoom in room.ConnectedRooms)
            {
                Debug.DrawLine(room.Positon, connectedRoom.Positon);
            }
        }
    }
}