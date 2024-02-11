using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class CaveGenerator : MonoBehaviour
{
    [Header("Generation Params")]
    [Range(0, 100)]
    [SerializeField] private int _noiseDensity;
    [SerializeField] private int _width;
    [SerializeField] private int _height;
    [SerializeField] private int _iterations;

    [Header("Tiles")]
    [SerializeField] private Tilemap _floorTilemap;
    [SerializeField] private TileBase _floorTile;
    [SerializeField] private TileBase _wallTile;
    [SerializeField] private TileBase _exampleFillTile;

    private bool[,] _caveGrid;
    private List<Vector2Int> _floorPositions;


    // Start is called before the first frame update
    void Start()
    {
        _floorPositions = new List<Vector2Int>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GenerateCave();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CellullarAutomataIteration(_caveGrid);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            RemoveDisconnectedSpaces();
        }

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            PaintTiles();
        }
    }

    #region Cave Generation
    private void GenerateCave()
    {
        bool[,] noiseGrid = GenerateNoise();

        bool completed = false;
        int i = 0;
        
        while (!completed) 
        {
            // Avoid any possible infinite loop
            if (i == 100)
            {
                Debug.Log("its over 9000");
                break;
            }

            if (CellullarAutomataIteration(noiseGrid)) completed = false;

            i++;
        }
        

        _caveGrid = noiseGrid;
        PaintTiles();
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
    private void RemoveDisconnectedSpaces()
    {
        int x = 0, y = 0;

        while (_caveGrid[x, y])
        {
            x++;
            y++;
        }

        bool[,] grid = _caveGrid;
        FloodTiles(x, y, grid);

        foreach(Vector2Int position in _floorPositions)
        {
            var gridPos = _floorTilemap.WorldToCell(new Vector3Int(position.x, position.y));
            _floorTilemap.SetTile(gridPos, _exampleFillTile);
        }
    }

    private void FloodTiles(int x, int y, bool[,] grid)
    {
        if (x >= 0 && x < _width && y >= 0 && y < _height)
        {
            if (!grid[x, y])
            {
                grid[x, y] = true;
                _floorPositions.Add(new Vector2Int(x, y));

                FloodTiles(x + 1, y, grid);
                FloodTiles(x - 1, y, grid);
                FloodTiles(x, y + 1, grid);
                FloodTiles(x, y - 1, grid);
            }
        }
    }

    private void Show()
    {

    }

    #endregion

    private void PaintTiles()
    {
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                Vector3Int gridPosition = _floorTilemap.WorldToCell(new Vector3Int(x, y));

                if (_caveGrid[x, y]) _floorTilemap.SetTile(gridPosition, _wallTile);
                else _floorTilemap.SetTile(gridPosition, _floorTile);
            }
        }
    }
}
