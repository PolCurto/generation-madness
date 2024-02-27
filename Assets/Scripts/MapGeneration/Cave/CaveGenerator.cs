using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class CaveGenerator : MonoBehaviour
{
    [SerializeField] private CaveLogic _caveLogic;
    [SerializeField] private TilesController _tilesController;
    [SerializeField] private GameObject _grid;

    [Header("Generation Params")]
    [Range(0, 100)]
    [SerializeField] private int _noiseDensity;
    [SerializeField] private int _width;
    [SerializeField] private int _height;
    [SerializeField] private int _iterations;
    [SerializeField] private int _minFloorTiles;

    [Header("Tiles")]
    [SerializeField] private Tilemap _floorTilemap;
    [SerializeField] private Tilemap _wallsTilemap;
    [SerializeField] private TileBase _floorTile;
    [SerializeField] private TileBase _exampleFillTile;

    private bool[,] _caveGrid;
    private FloorGrid _floorGrid;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GenerateCave();
        }
    }

    #region Cave Generation
    private void GenerateCave()
    {
        _floorGrid = new FloorGrid(_width, _height);
        _floorTilemap.ClearAllTiles();
        _wallsTilemap.ClearAllTiles();

        bool[,] noiseGrid = GenerateNoise();

        int i = 0;
        
        while (i < _iterations) 
        {
            if (CellullarAutomataIteration(noiseGrid)) break;
            i++;
        }
        
        _caveGrid = noiseGrid;
        DefineFinalShape();
        PaintTiles();
        _caveLogic.SetFloorGrid(_floorGrid);
        CenterCave();
    }

    /// <summary>
    /// Generates the noise matrix. True = Wall | False = Floor
    /// </summary>
    /// <returns></returns>
    private bool[,] GenerateNoise()
    {
        bool[,] noiseGrid = new bool[_width, _height];

        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                if (x == 0 || x == _width - 1 || y == 0 || y == _height - 1)
                {
                    noiseGrid[x, y] = true;
                }
                else
                {
                    if (Random.Range(0, 100) < _noiseDensity) noiseGrid[x, y] = true;
                    else noiseGrid[x, y] = false;
                } 
            }
        }

        return noiseGrid;
    }

    private bool CellullarAutomataIteration(bool[,] noiseGrid)
    {
        bool[,] tempGrid = (bool[,])noiseGrid.Clone();

        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                int numWalls = CountNeighbourWalls(new Vector2Int(x, y), tempGrid);

                if (numWalls > 4)
                {
                    noiseGrid[x, y] = true;
                }
                else noiseGrid[x, y] = false;
            }
        }

        if (tempGrid == noiseGrid) return true;
        else return false;
    }

    private int CountNeighbourWalls(Vector2Int position, bool[,] grid)
    {
        int neighbourWalls = 0;

        for (int y = position.y - 1; y <= position.y + 1; y++)
        {
            for (int x = position.x - 1; x <= position.x + 1; x++)
            {
                if (IsWithinMapGrid(x, y))
                {
                    if (grid[x, y])
                    {
                        neighbourWalls++;
                    }
                }
                else 
                {
                    neighbourWalls++;
                }
            }
        }
        return neighbourWalls;
    }

    private bool IsWithinMapGrid(int x, int y)
    {
        if (x >= _width || x < 0 || y >= _height || y < 0) return false;
        else return true;
    }
    #endregion

    #region Cave Refinement
    private void DefineFinalShape()
    {
        int x = 0, y = 0;

        while (_caveGrid[x, y])
        {
            x++;
            y++;
        }

        bool[,] grid = _caveGrid;
        FloodTiles(x, y, grid);

        if (_floorGrid.GridPositions.Count < _minFloorTiles)
        {
            GenerateCave();
        }
    }

    private void FloodTiles(int x, int y, bool[,] grid)
    {
        if (x >= 0 && x < _width && y >= 0 && y < _height)
        {
            if (!grid[x, y])
            {
                grid[x, y] = true;
                GridPos gridPos = new GridPos(new Vector2Int(x, y));
                _floorGrid.GridPositions.Add(gridPos);

                FloodTiles(x + 1, y, grid);
                FloodTiles(x - 1, y, grid);
                FloodTiles(x, y + 1, grid);
                FloodTiles(x, y - 1, grid);
            }
        }
    }

    private void CenterCave()
    {
        Vector3 newPosition = Vector3.zero;
        newPosition.x -= _width / 2;
        newPosition.y -= _height / 2;
        _grid.transform.position = newPosition;
    }
    #endregion
    
    private void PaintTiles()
    {
        foreach(GridPos pos in _floorGrid.GridPositions)
        {
            _floorTilemap.SetTile((Vector3Int)pos.CellPosition, _floorTile);
        }
    }
}
