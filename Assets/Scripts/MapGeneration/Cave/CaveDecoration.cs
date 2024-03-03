using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using UnityEditor.Experimental.GraphView;

public class CaveDecoration : MonoBehaviour
{
    [SerializeField] private GameObject _sample;
    [SerializeField] private Tilemap _floorTilemap;
    [SerializeField] private int _chunkSize;

    [Header("Generation Options")]
    [SerializeField] private bool _generateByChunks;
    [SerializeField] private bool _setBorders;

    private FloorGrid _floorGrid;
    private SampleReader _reader;
    private List<Node> _nodes;
    private List<GridPos>[] _gridParts;
    private int _iterations = 0;
    private int _gridPartIndex = 0;
    private bool running = false;
    private bool finished = false;

    private readonly Vector2Int[] Surroundings = new Vector2Int[]
        {
            new Vector2Int (1, 0),      // Right
            new Vector2Int (0, -1),     // Down
            new Vector2Int (-1, 0),     // Left
            new Vector2Int (0, 1),      // Up
        };

    private void Start()
    {
        _reader = new SampleReader(_sample.GetComponentInChildren<Tilemap>());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Alpha2))
        {
            _nodes = _reader.GetNodesFromSample();

            /*
            int i = 1;
            int j = 1;
            int k = 1;
            int l = 1;
            int m = 1;
            
            foreach (Node node in _nodes) 
            {
                Debug.Log("Node " + i + " with tile: " + node.Tile);

                
                Debug.Log("Right neighbours: ");
                foreach (Node rightNode in node.RightNodes)
                {
                    Debug.Log("Right neigbour " + j + " with tile: " + rightNode.Tile);
                    j++;
                }

                Debug.Log("Down neighbours: ");
                foreach (Node rightNode in node.DownNodes)
                {
                    Debug.Log("Down neigbour " + k + " with tile: " + rightNode.Tile);
                    k++;
                }

                Debug.Log("Left neighbours: ");
                foreach (Node rightNode in node.LeftNodes)
                {
                    Debug.Log("Left neigbour " + l + " with tile: " + rightNode.Tile);
                    l++;
                }

                Debug.Log("Up neighbours: ");
                foreach (Node rightNode in node.UpNodes)
                {
                    Debug.Log("Up neigbour " + m + " with tile: " + rightNode.Tile);
                    m++;
                }
                
                j = 1;
                k = 1;
                l = 1;
                m = 1;
                
            i++;
            }
            */

            DivideGrid();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetPossibleNodes();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (_generateByChunks)
            {
                if (!running)
                {
                    running = true;
                    StartCoroutine(CheckEntropyChunk(_gridPartIndex));
                }
            }
            else
            {
                StartCoroutine(CheckEntropy());
            }
        }

        if (finished)
        {
            //Debug.Log("Go");
            finished = false;
            while (_gridParts[_gridPartIndex].Count == 0)
            {
                //Debug.Log("Empty");
                _gridPartIndex++;
            }
            StartCoroutine(CheckEntropyChunk(_gridPartIndex));
        }
    }

    private void DivideGrid()
    {
        //int arrayLength = Mathf.CeilToInt(_floorGrid.GridPositions.Count / 100f);
        _gridParts = new List<GridPos>[(_floorGrid.Height * _floorGrid.Width) / (_chunkSize * _chunkSize)];
        int index = 0;

        for (int y = -_floorGrid.Height / 2; y < _floorGrid.Height / 2; y++)
        {
            for (int x = -_floorGrid.Width / 2; x < _floorGrid.Width / 2; x++)
            {
                if (x % 20 == 0 && y % 20 == 0)
                {
                    _gridParts[index] = GetGridChunk(x, y);
                    Debug.Log(_gridParts[index].Count);
                    index++;
                }
            }
        }
    }

    private List<GridPos> GetGridChunk(int x, int y)
    {
        List<GridPos> chunk = new List<GridPos>();

        for (int i = y - _chunkSize / 2; i < y + _chunkSize / 2; i++)
        {
            for (int j = x - _chunkSize / 2; j < x + _chunkSize / 2; j++)
            {
                if (_floorGrid.TryGetGridPosFromWorld(new Vector2Int(i, j), out GridPos gridPos))
                {
                    chunk.Add(gridPos);
                }
            }
        }
        return chunk;
    }


    private void SetPossibleNodes()
    {
        if (_setBorders)
        {
            foreach (GridPos gridPos in _floorGrid.GridPositions)
            {
                if (gridPos.IsNearWall())
                {
                    gridPos.PossibleNodes = new List<Node> { _nodes[0] };
                    gridPos.Collapsed = true;
                    _floorTilemap.SetTile((Vector3Int)gridPos.CellPosition, _nodes[0].Tile);
                }
                else
                {
                    gridPos.PossibleNodes = _nodes;
                }
            }
            finished = true;
        }
        else
        {
            foreach (GridPos gridPos in _floorGrid.GridPositions)
            {
                gridPos.PossibleNodes = _nodes;
            }
        }

        int limit = _gridParts.Count();

        /*
        for (_gridPartIndex = 0; _gridPartIndex < limit; _gridPartIndex++)
        {
            StartCoroutine(CheckEntropy(_gridPartIndex, 0));
        } 
        */

    }

    private IEnumerator CheckEntropyChunk(int index)
    {
        List<GridPos> tempGrid = new List<GridPos>(_gridParts[index]);

        tempGrid.RemoveAll(c => c.Collapsed);
        int iterations = tempGrid.Count;

        // Orders the uncollapsed positions by their entropy (lowest possible tiles)
        tempGrid.Sort((a, b) => { return a.PossibleNodes.Count - b.PossibleNodes.Count; });

        //Debug.Log("Temp grid length: " + tempGrid.Count);
        //Debug.Log("Iterations: " + iterations);
        //Debug.Log("Index: " + index);
        int numOptions = tempGrid[0].PossibleNodes.Count;
        //Debug.Log("Num options: " +  numOptions);
        int stopIndex = default;

        // Gets the positions with the same entropy if there are
        for (int i = 1; i < tempGrid.Count; i++)
        {
            if (tempGrid[i].PossibleNodes.Count > numOptions)
            {
                stopIndex = i;
                break;
            }
        }

        // Removes the rest of positions
        if (stopIndex > 0)
        {
            tempGrid.RemoveRange(stopIndex, tempGrid.Count - stopIndex);
        }

        yield return null;

        CollapseCellChunk(tempGrid, index, iterations);
    }

    private void CollapseCellChunk(List<GridPos> tempGrid, int index, int iterations)
    {
        // Gets a random tile from the possible ones
        int randomIndex = UnityEngine.Random.Range(0, tempGrid.Count);
        GridPos posToCollapse = tempGrid[randomIndex];

        posToCollapse.Collapsed = true;

        // Debug.Log("Grid part: " + _gridPartIndex);
        //Debug.Log("Possible nodes: " + posToCollapse.PossibleNodes.Count);
        //Debug.Log("Position: " + posToCollapse.WorldPosition);

        //Node selectedNode = posToCollapse.PossibleNodes[UnityEngine.Random.Range(0, posToCollapse.PossibleNodes.Count)];
        Node selectedNode;
        if (posToCollapse.PossibleNodes.Count > 0)
        {
            selectedNode = posToCollapse.PossibleNodes[UnityEngine.Random.Range(0, posToCollapse.PossibleNodes.Count)];
        }
        else
        {
            //Debug.LogWarning("no nodes amigo");
            selectedNode = _nodes[0];
        }

        posToCollapse.PossibleNodes = new List<Node> { selectedNode };

        _floorTilemap.SetTile((Vector3Int)posToCollapse.CellPosition, selectedNode.Tile);

        UpdateGenerationChunk(index, iterations);
    }

    private void UpdateGenerationChunk(int gridIndex, int iterations)
    {
        int index = 0;

        foreach (GridPos gridPos in _gridParts[gridIndex])
        {
            if (!gridPos.Collapsed)
            {
                List<Node> options = new List<Node>(_nodes);

                // Check up node
                if (_floorGrid.TryGetGridPosFromCell(gridPos.CellPosition + Surroundings[3], out GridPos upPos))
                {
                    List<Node> validOptions = new List<Node>();
                    //Debug.Log("Up node position: " + upPos.WorldPosition);
                    //Debug.Log("Up node possibilities: " + upPos.PossibleNodes.Count);

                    foreach (Node possibleOption in upPos.PossibleNodes)
                    {
                        validOptions.AddRange(possibleOption.DownNodes);
                    }
                    ApplyNeighbourOptions(options, validOptions);
                }

                // Check right node
                if (_floorGrid.TryGetGridPosFromCell(gridPos.CellPosition + Surroundings[0], out GridPos upPos2))
                {
                    List<Node> validOptions = new List<Node>();
                    //Debug.Log("Right node position: " + upPos2.WorldPosition);
                    //Debug.Log("Right node possibilities: " + upPos2.PossibleNodes.Count);

                    foreach (Node possibleOption in upPos2.PossibleNodes)
                    {
                        validOptions.AddRange(possibleOption.LeftNodes);
                    }
                    ApplyNeighbourOptions(options, validOptions);
                }

                // Check down node
                if (_floorGrid.TryGetGridPosFromCell(gridPos.CellPosition + Surroundings[1], out GridPos upPos3))
                {
                    List<Node> validOptions = new List<Node>();
                    // Debug.Log("Down node position: " + upPos3.WorldPosition);
                    //Debug.Log("Down node possibilities: " + upPos3.PossibleNodes.Count);

                    foreach (Node possibleOption in upPos3.PossibleNodes)
                    {
                        validOptions.AddRange(possibleOption.UpNodes);
                    }
                    ApplyNeighbourOptions(options, validOptions);
                }

                // Check left node
                if (_floorGrid.TryGetGridPosFromCell(gridPos.CellPosition + Surroundings[2], out GridPos upPos4))
                {
                    List<Node> validOptions = new List<Node>();
                    //Debug.Log("Left node position: " + upPos4.WorldPosition);
                    //Debug.Log("Left node possibilities: " + upPos4.PossibleNodes.Count);
                    foreach (Node possibleOption in upPos4.PossibleNodes)
                    {
                        validOptions.AddRange(possibleOption.RightNodes);
                    }
                    ApplyNeighbourOptions(options, validOptions);
                }

                gridPos.PossibleNodes = options;
            }
            index++;
        }
        _iterations++;

        if (iterations > 1)
        {
            StartCoroutine(CheckEntropyChunk(gridIndex));
        }
        else
        {
            UpdateNeighbours(gridIndex);
            _gridPartIndex++;
            running = false;
            finished = true;
        }
    }

    private void UpdateNeighbours(int index)
    {
        foreach (GridPos gridPos in _gridParts[index])
        {
            foreach (GridPos neighbour in gridPos.Neighbours)
            {
                if (!neighbour.Collapsed)
                {
                    List<Node> options = new List<Node>(_nodes);

                    // Check up node
                    if (_floorGrid.TryGetGridPosFromCell(neighbour.CellPosition + Surroundings[3], out GridPos upPos))
                    {
                        List<Node> validOptions = new List<Node>();
                        //Debug.Log("Up node position: " + upPos.WorldPosition);
                        //Debug.Log("Up node possibilities: " + upPos.PossibleNodes.Count);

                        foreach (Node possibleOption in upPos.PossibleNodes)
                        {
                            validOptions.AddRange(possibleOption.DownNodes);
                        }
                        ApplyNeighbourOptions(options, validOptions);
                    }

                    // Check right node
                    if (_floorGrid.TryGetGridPosFromCell(neighbour.CellPosition + Surroundings[0], out GridPos upPos2))
                    {
                        List<Node> validOptions = new List<Node>();
                        //Debug.Log("Right node position: " + upPos2.WorldPosition);
                        //Debug.Log("Right node possibilities: " + upPos2.PossibleNodes.Count);

                        foreach (Node possibleOption in upPos2.PossibleNodes)
                        {
                            validOptions.AddRange(possibleOption.LeftNodes);
                        }
                        ApplyNeighbourOptions(options, validOptions);
                    }

                    // Check down node
                    if (_floorGrid.TryGetGridPosFromCell(neighbour.CellPosition + Surroundings[1], out GridPos upPos3))
                    {
                        List<Node> validOptions = new List<Node>();
                        // Debug.Log("Down node position: " + upPos3.WorldPosition);
                        //Debug.Log("Down node possibilities: " + upPos3.PossibleNodes.Count);

                        foreach (Node possibleOption in upPos3.PossibleNodes)
                        {
                            validOptions.AddRange(possibleOption.UpNodes);
                        }
                        ApplyNeighbourOptions(options, validOptions);
                    }

                    // Check left node
                    if (_floorGrid.TryGetGridPosFromCell(neighbour.CellPosition + Surroundings[2], out GridPos upPos4))
                    {
                        List<Node> validOptions = new List<Node>();
                        //Debug.Log("Left node position: " + upPos4.WorldPosition);
                        //Debug.Log("Left node possibilities: " + upPos4.PossibleNodes.Count);
                        foreach (Node possibleOption in upPos4.PossibleNodes)
                        {
                            validOptions.AddRange(possibleOption.RightNodes);
                        }
                        ApplyNeighbourOptions(options, validOptions);
                    }

                    neighbour.PossibleNodes = options;
                }
            }
        }
    }

    private IEnumerator CheckEntropy()
    {
        List<GridPos> tempGrid = new List<GridPos>(_floorGrid.GridPositions);

        tempGrid.RemoveAll(c => c.Collapsed);

        // Orders the uncollapsed positions by their entropy (lowest possible tiles)
        tempGrid.Sort((a, b) => { return a.PossibleNodes.Count - b.PossibleNodes.Count; });

        int numOptions = tempGrid[0].PossibleNodes.Count;
        //Debug.Log("Num options: " +  numOptions);
        int stopIndex = default;

        // Gets the positions with the same entropy if there are
        for (int i = 1; i < tempGrid.Count; i++)
        {
            if (tempGrid[i].PossibleNodes.Count > numOptions)
            {
                stopIndex = i;
                break;
            }
        }

        // Removes the rest of positions
        if (stopIndex > 0)
        {
            tempGrid.RemoveRange(stopIndex, tempGrid.Count - stopIndex);
        }

        yield return null;

        CollapseCell(tempGrid);
    }

    private void CollapseCell(List<GridPos> tempGrid)
    {
        // Gets a random tile from the possible ones
        int randomIndex = UnityEngine.Random.Range(0, tempGrid.Count);
        GridPos posToCollapse = tempGrid[randomIndex];

        posToCollapse.Collapsed = true;

        //Debug.Log("Grid part: " + _gridPartIndex);
        //Debug.Log("Possible nodes: " + posToCollapse.PossibleNodes.Count);
        //Debug.Log("Position: " + posToCollapse.WorldPosition);

        Node selectedNode = posToCollapse.PossibleNodes[UnityEngine.Random.Range(0, posToCollapse.PossibleNodes.Count)];
        //Node selectedNode = posToCollapse.PossibleNodes.Count > 0 ? posToCollapse.PossibleNodes[UnityEngine.Random.Range(0, posToCollapse.PossibleNodes.Count)] : _nodes[0];
        posToCollapse.PossibleNodes = new List<Node> { selectedNode };

        _floorTilemap.SetTile((Vector3Int)posToCollapse.CellPosition, selectedNode.Tile);

        UpdateGeneration();
    }

    private void UpdateGeneration()
    {
        int index = 0;

        foreach (GridPos gridPos in _floorGrid.GridPositions)
        {
            //Debug.Log(HasCollapsedNeighbour(gridPos));
            if (!gridPos.Collapsed && HasCollapsedNeighbour(gridPos))
            {
                List<Node> options = new List<Node>(_nodes);

                // Check up node
                if (_floorGrid.TryGetGridPosFromCell(gridPos.CellPosition + Surroundings[3], out GridPos upPos))
                {
                    List<Node> validOptions = new List<Node>();
                    //Debug.Log("Up node position: " + upPos.WorldPosition);
                    //Debug.Log("Up node possibilities: " + upPos.PossibleNodes.Count);

                    foreach (Node possibleOption in upPos.PossibleNodes)
                    {
                        validOptions.AddRange(possibleOption.DownNodes);
                    }
                    ApplyNeighbourOptions(options, validOptions);
                }

                // Check right node
                if (_floorGrid.TryGetGridPosFromCell(gridPos.CellPosition + Surroundings[0], out GridPos upPos2))
                {
                    List<Node> validOptions = new List<Node>();
                    //Debug.Log("Right node position: " + upPos2.WorldPosition);
                    //Debug.Log("Right node possibilities: " + upPos2.PossibleNodes.Count);

                    foreach (Node possibleOption in upPos2.PossibleNodes)
                    {
                        validOptions.AddRange(possibleOption.LeftNodes);
                    }
                    ApplyNeighbourOptions(options, validOptions);
                }

                // Check down node
                if (_floorGrid.TryGetGridPosFromCell(gridPos.CellPosition + Surroundings[1], out GridPos upPos3))
                {
                    List<Node> validOptions = new List<Node>();
                    //Debug.Log("Down node position: " + upPos3.WorldPosition);
                    //Debug.Log("Down node possibilities: " + upPos3.PossibleNodes.Count);

                    foreach (Node possibleOption in upPos3.PossibleNodes)
                    {
                        validOptions.AddRange(possibleOption.UpNodes);
                    }
                    ApplyNeighbourOptions(options, validOptions);
                }

                // Check left node
                if (_floorGrid.TryGetGridPosFromCell(gridPos.CellPosition + Surroundings[2], out GridPos upPos4))
                {
                    List<Node> validOptions = new List<Node>();
                    //Debug.Log("Left node position: " + upPos4.WorldPosition);
                    //Debug.Log("Left node possibilities: " + upPos4.PossibleNodes.Count);
                    foreach (Node possibleOption in upPos4.PossibleNodes)
                    {
                        validOptions.AddRange(possibleOption.RightNodes);
                    }
                    ApplyNeighbourOptions(options, validOptions);
                }

                gridPos.PossibleNodes = options;
            }
            index++;
        }
        _iterations++;

        if (_iterations < _floorGrid.GridPositions.Count)
        {
            StartCoroutine(CheckEntropy());
        }
    }

    private void ApplyNeighbourOptions(List<Node> optionsList, List<Node> neighbourOptions)
    {
        for (int x = optionsList.Count - 1; x >= 0; x--)
        {
            var element = optionsList[x];
            if (!neighbourOptions.Contains(element))
            {
                optionsList.RemoveAt(x);
            }
        }
    }

    private bool HasCollapsedNeighbour(GridPos gridPos)
    {

        foreach (GridPos neighbour in gridPos.Neighbours)
        {
            if (neighbour.Collapsed) return true;
        }



        return false;

    }

    public void SetFloorGrid(FloorGrid floorGrid)
    {
        _floorGrid = floorGrid;
    }
}