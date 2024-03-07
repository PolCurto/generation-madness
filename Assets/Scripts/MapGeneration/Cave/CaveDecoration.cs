using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CaveDecoration : MonoBehaviour
{
    [SerializeField] private WFC _wfc;
    [SerializeField] private TileBase _debugTile;
    [SerializeField] private Tilemap _detailsTilemap;
    [SerializeField] private TilesController _tilesController;

    [Header("Wall Props")]
    [SerializeField] private GameObject[] _wallProps;
    [Range(0f, 1f)]
    [SerializeField] private float _wallPropRate;

    [Header("Ground Props")]
    [SerializeField] private GameObject[] _groundProps;
    [Range(0f, 1f)]
    [SerializeField] private float _groundPropRate;

    private FloorGrid _floorGrid;
    private bool generate;

    void Start()
    {
        _wfc = GetComponent<WFC>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            //GenerateGroundTiles();
            //_wfc.SetFloorGrid(_floorGrid);
            //_wfc.GetNodesFromSample();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            GenerateGroundTiles();
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            PlaceWallProps();
            PlaceGroundProps();
        }
    }

    private void GenerateGroundTiles()
    {
        if (generate) return;
        generate = true;

        _wfc.SetFloorGrid(_floorGrid);
        _wfc.GetNodesFromSample();
        _wfc.GenerateFloorTiles();
    }

    #region Props
    private void PlaceWallProps()
    {
        Vector2Int[] up = new Vector2Int[] { Vector2Int.up };
        List<GridPos> availablePositions = GetSuitablePropPositions(up, true);

        foreach (GridPos pos in availablePositions)
        {
            if (Random.Range(0f, 1f) < _wallPropRate)
            {
                GameObject wallProp = Instantiate(_wallProps[Random.Range(0, _wallProps.Length)], (Vector3Int)pos.WorldPosition, Quaternion.identity);
                _tilesController.SimplePrefabToMainGrid(wallProp, _detailsTilemap);
            }
        }
    }

    private void PlaceGroundProps()
    {
        Vector2Int[] positions = new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.right, Vector2Int.left };
        List<GridPos> availablePositions = GetSuitablePropPositions(positions, false);

        foreach (GridPos pos in availablePositions)
        {
            if (Random.Range(0f, 1f) < _groundPropRate)
            {
                Instantiate(_groundProps[Random.Range(0, _groundProps.Length)], (Vector3Int)pos.WorldPosition + new Vector3(0.5f, 0.5f), Quaternion.identity);
            }
        }
    }

    private List<GridPos> GetSuitablePropPositions(Vector2Int[] neighbourPositions, bool wallProp)
    {
        List<GridPos> availablePositions = new List<GridPos>();

        foreach (GridPos pos in _floorGrid.GridPositions)
        {
            if (wallProp)
            {
                if (!pos.HasNeighbourInPositions(neighbourPositions))
                {
                    availablePositions.Add(pos);
                }
            }
            else
            {
                if (pos.HasNeighbourInPositions(neighbourPositions))
                {
                    availablePositions.Add(pos);
                }
            }
            
        }

        return availablePositions;
    }
    #endregion

    public void SetFloorGrid(FloorGrid floorGrid)
    {
        _floorGrid = floorGrid;
    }
}
