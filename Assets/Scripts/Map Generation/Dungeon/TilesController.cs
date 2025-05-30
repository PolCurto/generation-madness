using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class TilesController : MonoBehaviour
{
    [Header("Tilemaps")]
    [SerializeField] private Tilemap _floorTilemap;
    [SerializeField] private Tilemap _wallTilemap;
    [SerializeField] private Tilemap _detailsTilemap;
    [SerializeField] private Tilemap _holesTilemap;
    [SerializeField] private Tilemap _obstaclesTilemap;

    [Header("Minimap")]
    [SerializeField] private Tilemap _minimapFloor;
    [SerializeField] private Tilemap _minimapWalls;
    [SerializeField] private Tilemap _minimapHoles;

    [Header("Tiles")]
    [SerializeField] private TileBase _floorTile;
    [SerializeField] private TileBase _wallTile;
    [SerializeField] private TileBase _limitTile;
    [SerializeField] private TileBase _minimapFloorTile;
    [SerializeField] private TileBase _minimapWallsTile;
    [SerializeField] private TileBase _minimapHolesTile;

    [Header("Wall Parameters")]
    [SerializeField] private int _tileRange;

    public Vector3Int[] _specialSurroundings = new Vector3Int[]
    {
        new Vector3Int (1, 0, 0),       // Right
        new Vector3Int (1, -1, 0),      // Bottom - Right
        new Vector3Int (0, -1, 0),      // Bottom
        new Vector3Int (-1, -1, 0),     // Bottom - Left
        new Vector3Int (-1, 0, 0),      // Left
        new Vector3Int (-1, 1, 0),      // Top - Left
        new Vector3Int (0, 1, 0),       // Top
        new Vector3Int (1, 1, 0),       // Top - Right
        new Vector3Int (0, -2, 0),      // Special case
        new Vector3Int (1, -2, 0),      // Special case
        new Vector3Int (-1, -2, 0)      // Special case
    };

    private Vector2Int[] surroundings = new Vector2Int[]
    {
        new Vector2Int (1, 0),
        new Vector2Int (-1, 0),
        new Vector2Int (0, 1),
        new Vector2Int (0, -1)
    };

    /// <summary>
    /// Copies the rooms tilemaps to the general level grid
    /// </summary>
    /// <param name="rooms"></param>
    public void GetRoomsToMainGrid(List<DungeonRoom> rooms)
    {
        foreach (DungeonRoom room in rooms)
        {
            Tilemap[] roomTilemaps = room.SceneRoom.GetComponentsInChildren<Tilemap>();

            roomTilemaps[1].CompressBounds();
            room.Width = roomTilemaps[1].size.x;
            room.Height = roomTilemaps[1].size.y;

            PassOnTiles(roomTilemaps[0], _floorTilemap);
            PassOnTiles(roomTilemaps[1], _wallTilemap);
            PassOnTiles(roomTilemaps[2], _detailsTilemap);

            if (roomTilemaps.Length > 3)
            {
                PassOnTiles(roomTilemaps[3], _holesTilemap);
            }

            if (roomTilemaps.Length > 4)
            {
                PassOnTiles(roomTilemaps[4], _obstaclesTilemap);
            }

            Destroy(room.SceneRoom);
        }
    }

    public void GetRoomsToMainGrid(List<TempleRoom> rooms)
    {
        foreach (TempleRoom room in rooms)
        {
            Tilemap[] roomTilemaps = room.SceneRoom.GetComponentsInChildren<Tilemap>();

            roomTilemaps[0].CompressBounds();
            roomTilemaps[1].CompressBounds();

            PassOnTiles(roomTilemaps[0], _floorTilemap);
            PassOnTiles(roomTilemaps[1], _wallTilemap);
            PassOnTiles(roomTilemaps[2], _detailsTilemap);

            if (roomTilemaps.Length > 3)
            {
                PassOnTiles(roomTilemaps[3], _holesTilemap);
            }

            Destroy(room.SceneRoom);
        }
    }

    public void PrefabToMainGrid(GameObject prefab)
    {
        Tilemap[] objectTilemaps = prefab.GetComponentsInChildren<Tilemap>();

        PassOnTiles(objectTilemaps[0], _floorTilemap);
        PassOnTiles(objectTilemaps[1], _wallTilemap);
        PassOnTiles(objectTilemaps[2], _detailsTilemap);
        Destroy(prefab);
    }

    public void SimplePrefabToMainGrid(GameObject prefab, Tilemap tilemap)
    {
        Tilemap objectTilemap = prefab.GetComponentInChildren<Tilemap>();

        PassOnTiles(objectTilemap, tilemap);
        Destroy(prefab);
    }

    /// <summary>
    /// Gets the tiles of the given tilemap and copies them to the same world position in the destination tilemap
    /// </summary>
    /// <param name="originTilemap">Tilemap to copy tiles from</param>
    /// <param name="destinationTilemap">Tilemap to copy tiles to</param>
    public void PassOnTiles(Tilemap originTilemap, Tilemap destinationTilemap)
    {
        originTilemap.CompressBounds();

        foreach (Vector3Int localPos in originTilemap.cellBounds.allPositionsWithin)
        {
            if (originTilemap.HasTile(localPos))
            {
                TileBase tile = originTilemap.GetTile(localPos);
                Vector3 worldPos = originTilemap.CellToWorld(localPos);
                Vector3Int finalPos = destinationTilemap.WorldToCell(worldPos);
                destinationTilemap.SetTile(finalPos, tile);
            }
        }
    }

    /// <summary>
    /// Draws the corridors floor tiles in the calculated positions, and adds an extra tile to make them have 2 tiles of width
    /// </summary>
    /// <param name="corridors"></param>
    public void DrawCorridors(List<Corridor> corridors)
    {
        int i;
        foreach(Corridor corridor in corridors)
        {
            i = 0;
            foreach(Vector2Int position in corridor.Positions)
            {
                Vector3Int localPos = _floorTilemap.WorldToCell(new Vector3Int(position.x, position.y));
                if (!_floorTilemap.HasTile(localPos))
                {
                    bool horizontal = corridor.Orientation[i];
                    Vector3Int auxPos = horizontal ? _floorTilemap.WorldToCell(new Vector3Int(position.x, position.y - 1)) : _floorTilemap.WorldToCell(new Vector3Int(position.x - 1, position.y));

                    _floorTilemap.SetTile(localPos, _floorTile);
                    _floorTilemap.SetTile(auxPos, _floorTile);

                    // Removes the walls where corridors are being drown
                    if (_wallTilemap.HasTile(localPos)) _wallTilemap.SetTile(localPos, null);
                    if (_wallTilemap.HasTile(auxPos)) _wallTilemap.SetTile(auxPos, null);

                    // Removes details where corridors are being drawn
                    if (_detailsTilemap.HasTile(localPos)) _detailsTilemap.SetTile(localPos, null);
                    if (_detailsTilemap.HasTile(auxPos)) _detailsTilemap.SetTile(auxPos, null);

                    // Removes the holes where corridors are being drown
                    if (_holesTilemap.HasTile(localPos)) _holesTilemap.SetTile(localPos, null);
                    if (_holesTilemap.HasTile(auxPos)) _holesTilemap.SetTile(auxPos, null);
                }
                i++;
            }
        }
    }

    /// <summary>
    /// Draws the walls to the map to cover the corridors
    /// </summary>
    public void DrawWalls(TileBase wallTile)
    {
        // T� molt marge de millora per� tarda molt menys
        foreach (Vector3Int floorTilePos in _floorTilemap.cellBounds.allPositionsWithin)
        {
            if (_floorTilemap.HasTile(floorTilePos))
            {
                for (int x = floorTilePos.x - _tileRange; x <= floorTilePos.x + _tileRange; x++)
                {
                    for (int y = floorTilePos.y - _tileRange; y <= floorTilePos.y + _tileRange; y++)
                    {
                        Vector3Int position = new Vector3Int(x, y);
                        if (!_floorTilemap.HasTile(position) && !_holesTilemap.HasTile(position)) _wallTilemap.SetTile(new Vector3Int(x, y), wallTile);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Covers the outer borders with empty tiles to merge with the background
    /// </summary>
    public void CleanWalls()
    {
        foreach (Vector3Int wallTilePos in _wallTilemap.cellBounds.allPositionsWithin)
        {
            if (_wallTilemap.HasTile(wallTilePos))
            {
                if (!CheckSurroundings(wallTilePos))
                {
                    _detailsTilemap.SetTile(wallTilePos, _limitTile);
                }
            }
        }
    }

    /// <summary>
    /// Checks if there are tiles surrounding the given position
    /// </summary>
    /// <param name="position">Reference position</param>
    /// <returns></returns>
    private bool CheckSurroundings(Vector3Int position)
    {
        foreach (Vector3Int offset in _specialSurroundings)
        {
            if (!_wallTilemap.HasTile(position + offset) && !_floorTilemap.HasTile(position + offset) && !_holesTilemap.HasTile(position + offset))
            {
                return false;
            }
        }
        return true;
    }

    public void RemoveWallTileAt(Vector3 position)
    {
        position.x -= 0.5f;
        position.y -= 0.5f;
        Vector3Int gridPos = _wallTilemap.WorldToCell(Vector3Int.RoundToInt(position));

        _wallTilemap.SetTile(gridPos, null);
    }

    #region Minimap
    public void SetMinimap()
    {
        foreach (Vector3Int floorTilePos in _floorTilemap.cellBounds.allPositionsWithin)
        {
            if (_floorTilemap.HasTile(floorTilePos))
            {
                _minimapFloor.SetTile(floorTilePos, _minimapFloorTile);
            }
        }

        foreach (Vector3Int wallTilePos in _wallTilemap.cellBounds.allPositionsWithin)
        {
            if (_wallTilemap.HasTile(wallTilePos) && ShowWallInMinimap(wallTilePos))
            {
                _minimapWalls.SetTile(wallTilePos, _minimapWallsTile);
            }
        }

        foreach (Vector3Int holeTilePos in _holesTilemap.cellBounds.allPositionsWithin)
        {
            if (_holesTilemap.HasTile(holeTilePos))
            {
                _minimapHoles.SetTile(holeTilePos, _minimapHolesTile);
            }
        }
    }

    private bool ShowWallInMinimap(Vector3Int position)
    {
        foreach (Vector3Int offset in surroundings)
        {
            if (_floorTilemap.HasTile(position + offset) || _holesTilemap.HasTile(position + offset))
            {
                return true;
            }
        }
        return false;
    }
    #endregion

    public Tilemap FloorTilemap => _floorTilemap;
    public Tilemap WallsTilemap => _wallTilemap;
    public Tilemap DetailsTilemap => _detailsTilemap;
    public Tilemap HolesTilemap => _holesTilemap;
    public Tilemap ObstaclesTilemap => _obstaclesTilemap;
}