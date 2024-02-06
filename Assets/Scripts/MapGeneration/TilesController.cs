using SuperTiled2Unity;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilesController : MonoBehaviour
{
    [SerializeField] private Tilemap _floorTilemap, _wallTilemap;
    [SerializeField] private TileBase _floorTile;

    /// <summary>
    /// Copies the rooms tilemaps to the general level grid
    /// </summary>
    /// <param name="rooms"></param>
    public void GetRoomsToMainGrid(List<Room> rooms)
    {
        foreach (Room room in rooms)
        {
            Tilemap[] roomTilemaps = room.SceneRoom.GetComponentsInChildren<Tilemap>();
            PassOnTiles(roomTilemaps[0], _floorTilemap);
            PassOnTiles(roomTilemaps[1], _wallTilemap);
            Destroy(room.SceneRoom);
        }
    }

    /// <summary>
    /// Gets the tiles of the given tilemap and copies them to the same world position in the destination tilemap
    /// </summary>
    /// <param name="originTilemap">Tilemap to copy tiles from</param>
    /// <param name="destinationTilemap">Tilemap to copy tiles to</param>
    private void PassOnTiles(Tilemap originTilemap, Tilemap destinationTilemap)
    {
        originTilemap.CompressBounds();
        BoundsInt roomBounds = originTilemap.cellBounds;

        foreach (Vector3Int localPos in originTilemap.cellBounds.allPositionsWithin)
        {
            Vector3 worldPos = originTilemap.CellToWorld(localPos);
            if (originTilemap.HasTile(localPos))
            {
                TileBase tile = originTilemap.GetTile(localPos);
                Vector3Int finalPos = destinationTilemap.WorldToCell(worldPos);
                destinationTilemap.SetTile(finalPos, tile);
            }
        }
    }

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

                    if (_wallTilemap.HasTile(localPos)) _wallTilemap.SetTile(localPos, null);
                    if (_wallTilemap.HasTile(auxPos)) _wallTilemap.SetTile(auxPos, null);
                }
                i++;
            }
        }

    }
}
