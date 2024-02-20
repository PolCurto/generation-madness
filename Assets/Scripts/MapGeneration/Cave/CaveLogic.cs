using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CaveLogic : MonoBehaviour
{
    [Header("Areas")]
    [SerializeField] private GameObject _startArea;
    [SerializeField] private GameObject _bossRoom;
    [SerializeField] private GameObject _weaponArea;
    [SerializeField] private GameObject _treasureArea;

    [Header("Objects")]
    [SerializeField] private GameObject _ammoChest;
    [SerializeField] private GameObject _healingChest;

    [Header("Enemies")]
    [SerializeField] private GameObject _enemyZone;
    [SerializeField] private GameObject[] _enemies;
 
    [Header("Cave Logic Parameters")]
    [SerializeField] private int _startPositionArea;
    [SerializeField] private int _chestsMinOffset;
    [SerializeField] private int _chestsMaxOffset;
    [SerializeField] private int _numAmmoChests;
    [SerializeField] private int _numHealingChests;
    [SerializeField] private int _minEnemiesDistance;

    [Header("Tiles")]
    [SerializeField] private TilesController _tilesController;
    [SerializeField] private Tilemap _floorTilemap;
    [SerializeField] private TileBase _wallTile;

    private int _maxDepth;
    private bool _startPointIsSet;
    private Vector2Int _worldStartPoint;
    private Vector2Int _gridStartPoint;
    private List<GridPos> _gridPositions;
    private List<GameObject> _chests;
    private List<EnemyZone> _enemyZones;
    private Vector3 _cellToWorldOffset;

    private void Start()
    {
        _gridPositions = new List<GridPos>();
        _chests = new List<GameObject>();
        _enemyZones = new List<EnemyZone>();
        _cellToWorldOffset = new Vector3(0.5f, 0.5f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetStartingPoint();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetTilesDepth();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SetSpecialZones();
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SetEnemyZones();
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            Spawn();
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            _tilesController.DrawWalls(_wallTile);
            _tilesController.CleanWalls();
        }
    }

    #region Basic Logic
    /// <summary>
    /// Sets an starting point in the generated map as close as it can to the (0, 0)  position
    /// </summary>
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

        var startArea = Instantiate(_startArea, (Vector3Int)_worldStartPoint, Quaternion.identity);
        _tilesController.PrefabToMainGrid(startArea);

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
                }
            }
        }

        visitedTilesDepth.OrderByDescending(x => x.Value);

        foreach (KeyValuePair<Vector2Int, int> depth in visitedTilesDepth)
        {
            _gridPositions.Add(new GridPos(depth.Value, depth.Key));
        }

        Debug.Log("Depth set");
    }
    #endregion

    #region Special Zones
    private void SetSpecialZones()
    {
        SetBoss();
        Vector3Int treasurePosition = SetTreasurePoint();
        SetWeaponPoint(treasurePosition);

        for (int i = 0; i < _numAmmoChests; i++) 
        {
            SetChest(_ammoChest);
        }

        for (int i = 0; i < _numHealingChests; i++)
        {
            SetChest(_healingChest);
        }
    }

    /// <summary>
    /// Sets the boss zone in the deepest tile of the generated map
    /// </summary>
    private void SetBoss()
    {
        Vector3 bossPosition = _floorTilemap.CellToWorld((Vector3Int)_gridPositions[^1].Position);
        var bossRoom = Instantiate(_bossRoom, bossPosition, Quaternion.identity);
        _tilesController.PrefabToMainGrid(bossRoom);
    }

    /// <summary>
    /// Sets the treasure zone in the furthes area regarding the boss
    /// </summary>
    private Vector3Int SetTreasurePoint()
    {
        _maxDepth = _gridPositions[^1].Depth;
        float oldDistance = 0;
        Vector2Int gridPosition = new Vector2Int();
        Debug.Log(_maxDepth);

        foreach(GridPos gridPos in _gridPositions)
        {
            if (gridPos.Depth > _gridPositions[^1].Depth * 0.75) break;

            float currDistance = Vector2Int.Distance(gridPos.Position, _gridPositions[^1].Position);
            if (currDistance > oldDistance)
            {
                oldDistance = currDistance;
                gridPosition = gridPos.Position;
            }
        }

        Vector3 worldPosition = _floorTilemap.CellToWorld((Vector3Int)gridPosition);
        Debug.Log("Treasure position: " + worldPosition);

        var treasureArea = Instantiate(_treasureArea, worldPosition, Quaternion.identity);
        _tilesController.PrefabToMainGrid(treasureArea);

        return Vector3Int.RoundToInt(worldPosition);
    }

    /// <summary>
    /// Sets the weapon zone in an isolated area regarding the boss and the treasure positions
    /// </summary>
    private void SetWeaponPoint(Vector3Int treasurePosition)
    {
        float oldDistance = 0;
        Vector2Int gridPosition = new Vector2Int();

        foreach (GridPos gridPos in _gridPositions)
        {
            if (gridPos.Depth > _gridPositions[^1].Depth * 0.5) break;

            float currDistance = Vector2Int.Distance(gridPos.Position, _gridPositions[^1].Position) + Vector2Int.Distance(gridPos.Position, (Vector2Int)_floorTilemap.WorldToCell(treasurePosition));
            if (currDistance > oldDistance)
            {
                oldDistance = currDistance;
                gridPosition = gridPos.Position;
            }
        }

        Vector3 worldPosition = _floorTilemap.CellToWorld((Vector3Int)gridPosition);
        Debug.Log("Weapon position: " + worldPosition);

        var weaponArea = Instantiate(_weaponArea, worldPosition, Quaternion.identity);
        _tilesController.PrefabToMainGrid(weaponArea);
    }

    /// <summary>
    /// Sets a chest randomly in the map
    /// </summary>
    private void SetChest(GameObject chest)
    {
        int tileDepth = _gridPositions[^1].Depth / 2 + Random.Range(_chestsMinOffset, _chestsMaxOffset);

        float oldDistance = 0;
        float currDistance;
        Vector2Int gridPosition = new Vector2Int();

        foreach (GridPos gridPos in _gridPositions)
        {
            if (gridPos.Depth >= tileDepth - 20 && gridPos.Depth <= tileDepth + 20)
            {
                if (_chests.Count == 0)
                {
                    gridPosition = gridPos.Position;
                    break;
                }
                currDistance = 0;
                foreach (GameObject c in _chests)
                {
                    currDistance += Vector2Int.Distance(gridPos.Position, (Vector2Int)_floorTilemap.WorldToCell(c.transform.position));
                }

                if (currDistance > oldDistance)
                {
                    oldDistance = currDistance;
                    gridPosition = gridPos.Position;
                }
            }
        }
        
        Vector3 worldPosition = _floorTilemap.CellToWorld((Vector3Int)gridPosition);
        worldPosition.x += 0.5f;
        worldPosition.y += 0.5f;
        _chests.Add(Instantiate(chest, worldPosition, Quaternion.identity));
    }
    #endregion

    #region Enemy Spawn
    private void SetEnemyZones()
    {
        /*
        foreach (GridPos gridPos in _gridPositions)
        {
            if (gridPos.Depth == 15 || gridPos.Depth == 30 || gridPos.Depth == 45 || gridPos.Depth == 60 || gridPos.Depth == 75 || gridPos.Depth == 90)
            {
                var a = Instantiate(_enemyZone, _floorTilemap.CellToWorld((Vector3Int)gridPos.Position) + new Vector3(0.5f, 0.5f), Quaternion.identity);
                float value = gridPos.Depth;
                a.GetComponent<SpriteRenderer>().color = new Color((255 - (value * 2)) / 255, (255 - (value * 2)) / 255, (255 - (value * 2)) / 255);
            }
        }
        */
        List<Vector2Int> enemyPositions = new List<Vector2Int>();

        foreach (GridPos gridPos in _gridPositions)
        {
            if (gridPos.Depth <= 20 || !IsValidArea(1, gridPos.Position) || Vector2Int.Distance(gridPos.Position, _gridPositions[^1].Position) < 20) continue;

            bool isValid = true;

            foreach (EnemyZone zone in _enemyZones)
            {
                if (Vector2Int.Distance(gridPos.Position, zone.Position) <= _minEnemiesDistance) isValid = false;
            }

            if (isValid)
            {
                Instantiate(_enemyZone, _floorTilemap.CellToWorld((Vector3Int)gridPos.Position) + _cellToWorldOffset, Quaternion.identity);

                float depthLimit = _maxDepth / 3;
                EnemyZone.ZoneType type = 0;

                if (gridPos.Depth <= depthLimit) type = EnemyZone.ZoneType.Easy;
                else if (gridPos.Depth > depthLimit && gridPos.Depth <= depthLimit * 2) type = EnemyZone.ZoneType.Medium;
                else if (gridPos.Depth > depthLimit) type = EnemyZone.ZoneType.Hard;

                _enemyZones.Add(new EnemyZone(type, gridPos.Position, 7));
            }
        }
        /*
        foreach (EnemyZone zone in _enemyZones)
        {
            switch (zone.Type)
            {
                case EnemyZone.ZoneType.Easy:
                    SpawnEnemies(SetEnemyPool(5));
                    break;
                case EnemyZone.ZoneType.Medium:
                    SpawnEnemies(SetEnemyPool(10));
                    break;
                case EnemyZone.ZoneType.Hard:
                    SpawnEnemies(SetEnemyPool(15));
                    break;
            }
        }
        */
    }

    private void Spawn()
    {
        List<Enemy> availableEnemies = new List<Enemy>();

        foreach (GameObject enemy in _enemies)
        {
            availableEnemies.Add(enemy.GetComponent<Enemy>());
        }

        foreach (EnemyZone zone in _enemyZones)
        {
            switch (zone.Type)
            {
                case EnemyZone.ZoneType.Easy:
                   // SpawnEnemies(zone, SetEnemyPool(5, availableEnemies));
                    break;
                case EnemyZone.ZoneType.Medium:
                    //SpawnEnemies(zone, SetEnemyPool(10, availableEnemies));
                    break;
                case EnemyZone.ZoneType.Hard:
                    SpawnEnemies(zone, SetEnemyPool(15, availableEnemies));
                    break;
            }
        }
    }

    private List<Enemy> SetEnemyPool(int enemyPoints, List<Enemy> availableEnemies)
    {
        List<Enemy> enemiesToSpawn = new List<Enemy>();
        
        while (enemyPoints > 0)
        {
            var enemy = availableEnemies[Random.Range(0, 2)];

            if (enemy.Cost() <= enemyPoints)
            {
                enemyPoints -= enemy.Cost();
                enemiesToSpawn.Add(enemy);
            }
        }

        return enemiesToSpawn;
    }

    private void SpawnEnemies(EnemyZone zone, List<Enemy> enemies)
    {
        Vector2Int center = zone.Position;
        int offset = Mathf.RoundToInt(zone.Area / 2);

        int numEnemies = enemies.Count;
        int i = 0;

        for (int y = center.y - offset; y <= center.y + offset; y += 2)
        {
            for (int x = center.x - offset; x <= center.x + offset; x ++)
            {
                Vector3Int gridPosition = new Vector3Int(x, y);
                if (_floorTilemap.HasTile(gridPosition))
                {
                    Instantiate(enemies[i].gameObject, _floorTilemap.CellToWorld(new Vector3Int(x, y)) + _cellToWorldOffset, Quaternion.identity);
                    i++;
                    x++;
                    numEnemies--;
                    if (numEnemies <= 0) return;
                }
            }
        }
    }
    #endregion

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
