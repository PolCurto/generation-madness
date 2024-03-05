using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CaveDecoration : MonoBehaviour
{
    [SerializeField] private WFC _wfc;
    [SerializeField] private TileBase _debugTile;
    [SerializeField] private Tilemap _detailsTilemap;

    [Header("Props")]
    [SerializeField] private GameObject[] _wallProps;

    private FloorGrid _floorGrid;

    // Start is called before the first frame update
    void Start()
    {
        _wfc = GetComponent<WFC>();
    }

    private void GeneratoGroundTIles()
    {
        _wfc.GetNodesFromSample();
        // etc.
    }

    #region Wall Props
    private void PlaceWallProps()
    {
        List<GridPos> availablePositions = GetSuitableGridPositions();

        foreach (GridPos pos in availablePositions)
        {
            _detailsTilemap.SetTile((Vector3Int)pos.CellPosition, _debugTile);
        }
    }

    private List<GridPos> GetSuitableGridPositions()
    {
        List<GridPos> availablePositions = new List<GridPos>();
        Vector2Int up = Vector2Int.up;

        foreach (GridPos pos in _floorGrid.GridPositions) 
        {
            if (!pos.HasNeighbourInPosition(up))
            {
                availablePositions.Add(pos);
            }
        }

        return availablePositions;
    }
    #endregion

    private void SetFloorGrid(FloorGrid floorGrid)
    {
        _floorGrid = floorGrid;
    }
}
