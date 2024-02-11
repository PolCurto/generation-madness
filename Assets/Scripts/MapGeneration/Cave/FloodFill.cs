using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FloodFill : MonoBehaviour
{
    private List<Vector2Int> _floorPositions;

    // Start is called before the first frame update
    void Start()
    {
        _floorPositions = new List<Vector2Int>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FloodTiles(int x, int y, bool[,] grid)
    {
        if (!grid[x, y]) return;

        _floorPositions.Add(new Vector2Int(x, y));
        FloodTiles(x + 1, y, grid);
        FloodTiles(x - 1, y, grid);
        FloodTiles(x, y + 1, grid);
        FloodTiles(x, y - 1, grid);
    }
}