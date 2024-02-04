using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilesController : MonoBehaviour
{
    [SerializeField] private Tilemap _floorTilemap, _wallTilemap;

    private int _roomWidth = 30;
    private int _roomHeight = 20;

    public void GetRoomsToMainGrid(List<Room> rooms)
    {
        Vector2Int startPosition;

        foreach (Room room in rooms)
        {
            startPosition = Vector2Int.RoundToInt(room.Position);
            startPosition.x -= _roomWidth / 2;
            startPosition.y -= _roomHeight / 2;

            Tilemap[] roomTilemaps = room.SceneRoom.GetComponentsInChildren<Tilemap>();
            PassOnTiles(roomTilemaps[0], _floorTilemap, startPosition);
            PassOnTiles(roomTilemaps[1], _wallTilemap, startPosition);
        }
    }

    private void PassOnTiles(Tilemap originTilemap, Tilemap destinationTilemap, Vector2Int position)
    {
        originTilemap.CompressBounds();

        BoundsInt roomBounds = originTilemap.cellBounds;
        TileBase[] roomTiles = originTilemap.GetTilesBlock(roomBounds);

        Vector2Int basePosition = Vector2Int.zero;
        Debug.Log("Room bounds x: " + roomBounds.size.x + " y: " + roomBounds.size.y);

        foreach (var pos in originTilemap.cellBounds.allPositionsWithin)
        {
            Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
            Vector3 worldPlace = originTilemap.CellToWorld(localPlace);
            if (originTilemap.HasTile(localPlace))
            {
                TileBase tile = originTilemap.GetTile(localPlace);
                var place = destinationTilemap.WorldToCell(worldPlace);
                destinationTilemap.SetTile(place, tile);
            }
        }

        /*
        for (int x = 0; x < roomBounds.size.x; x++)
        {
            for (int y = 0; y < roomBounds.size.y; y++)
            {
                TileBase tile = roomTiles[x + y * roomBounds.size.x];

                if (tile != null)
                {
                    Debug.Log("x:" + x + " y:" + y + " tile:" + tile.name);

                    basePosition.x = position.x + x;
                    basePosition.y = position.y + y;

                    var roomTilePosition = originTilemap.CellToWorld((Vector3Int)basePosition);
                    var tilePosition = destinationTilemap.WorldToCell((Vector3Int)basePosition);
                    destinationTilemap.SetTile(tilePosition, tile);
                }
            }
        }
        */

        /*
        for (int x = 0; x < roomBounds.size.x; x++)
        {
            for (int y = 0; y < roomBounds.size.y; y++)
            {
                basePosition.x = position.x + x;
                basePosition.y = position.y + y;

                var tilePosition = originTilemap.WorldToCell((Vector3Int)basePosition);
                TileBase tile = originTilemap.GetTile(tilePosition);

                if (tile != null)
                {
                    Debug.Log("x:" + x + " y:" + y + " tile:" + tile.name);
                    destinationTilemap.SetTile((Vector3Int)basePosition, tile);
                }
            }
        }
        */
    }
}
