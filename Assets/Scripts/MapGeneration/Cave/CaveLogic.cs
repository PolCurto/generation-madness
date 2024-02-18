using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CaveLogic : MonoBehaviour
{
    [SerializeField] private Tilemap _floorTilemap;
    [SerializeField] private TileBase _exampleTile;
    [SerializeField] private Tilemap _bossRoom;

    [Header("Cave Logic Parameters")]
    [SerializeField] private int _startPositionArea;

    private bool _startPointIsSet;
    private Vector2Int _worldStartPoint;
    private Vector2Int _gridStartPoint;
    private List<GridPos> _gridPositions;

    private void Start()
    {
        _gridPositions = new List<GridPos>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SetStartingPoint();
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SetTilesDepth();
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            SetBoss();
        }
    }

    private void SetStartingPoint()
    {
        Vector2Int startPosition = (Vector2Int) _floorTilemap.WorldToCell(Vector3Int.zero);
        Vector2Int offset = Vector2Int.right;

        if (IsValidArea(_startPositionArea, startPosition))
        {
            _startPointIsSet = true;
            _worldStartPoint = Vector2Int.zero;
            _gridStartPoint = startPosition;
        }

        int counter = 0;
        int multiplier = 1;

        while (!_startPointIsSet)
        {
            switch (counter)
            {
                case 0: offset = Vector2Int.right * multiplier; break;
                case 1: offset = Vector2Int.down * multiplier; break;
                case 2: offset = Vector2Int.left * multiplier; break;
                case 3: offset = Vector2Int.up * multiplier; break;
            }

            if (IsValidArea(_startPositionArea, startPosition + offset))
            {
                _startPointIsSet = true;
                _worldStartPoint = Vector2Int.zero + offset;
                _gridStartPoint = startPosition + offset;
            }

            counter++;

            if (counter == 4)
            {
                counter = 0;
                multiplier++;
            }
        }

        Debug.Log(_worldStartPoint);
    }

    /// <summary>
    /// Sets the distance of each tile regarding the start position
    /// </summary>
    private void SetTilesDepth()
    {
        Queue<Vector2Int> tilesToVisit = new Queue<Vector2Int>();
        Dictionary<Vector2Int, int> visitedTilesDepth = new Dictionary<Vector2Int, int>();

        tilesToVisit.Enqueue(_gridStartPoint);
        visitedTilesDepth.Add(_gridStartPoint, 0);

        while (tilesToVisit.Count > 0)
        {
            Vector2Int currentPos = tilesToVisit.Dequeue();

            foreach (Vector2Int neighbor in GetNeighbors(currentPos))
            {
                int depth = visitedTilesDepth[currentPos] + 1;

                if (!visitedTilesDepth.ContainsKey(neighbor) && !tilesToVisit.Contains(neighbor))
                {
                    visitedTilesDepth[neighbor] = depth;
                    tilesToVisit.Enqueue(neighbor);


                    //Tile tile = _exampleTile as Tile;
                    //tile.color = new Color(depth/255, 0, 0);
                    //_floorTilemap.SetTile((Vector3Int)neighbor, tile);
                }
            }
        }

        visitedTilesDepth.OrderByDescending(x => x.Value);

        foreach (KeyValuePair<Vector2Int, int> depth in visitedTilesDepth)
        {
            _gridPositions.Add(new GridPos(depth.Value, depth.Key));
        }
    }

    private void SetBoss()
    {
        Vector2Int bossPosition = _gridPositions[^1].Position;
        Debug.Log(_floorTilemap.CellToWorld((Vector3Int)bossPosition));

    }

    private void SetTreasurePoint()
    {

    }



    private List<Vector2Int> GetNeighbors(Vector2Int position)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        Vector2Int[] surroundings = new Vector2Int[]
        {
            new Vector2Int (1, 0),       // Right
            new Vector2Int (0, -1),      // Bottom
            new Vector2Int (-1, 0),      // Left
            new Vector2Int (0, 1),       // Top
        };

        foreach (Vector2Int offset in surroundings)
        {
            if (_floorTilemap.HasTile((Vector3Int)position + (Vector3Int)offset))
            {
                neighbors.Add(position + offset);
            }
        }
        return neighbors;
    }

    private bool IsValidArea(int offset, Vector2Int position)
    {
        for (int y = position.y - offset; y <= position.y + offset; y++)
        {
            for (int x = position.x - offset; x <= position.x + offset; x++)
            {
                if (!_floorTilemap.HasTile(new Vector3Int(x, y))) return false;
            }
        }
        return true;
    }
}
