using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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

    private bool[,] _caveGrid;


    // Start is called before the first frame update
    void Start()
    {
        
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

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            PaintTiles();
        }
    }

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
                if (Random.Range(0, 100) < _noiseDensity) noiseGrid[x, y] = true;
                else noiseGrid[x, y] = false;
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
                if (CountNeighbourWalls(new Vector2Int(x, y), tempGrid) > 4) noiseGrid[x, y] = true;
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
