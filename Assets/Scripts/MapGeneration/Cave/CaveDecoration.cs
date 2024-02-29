using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class CaveDecoration : MonoBehaviour
{
    [SerializeField] private GameObject _sample;
    [SerializeField] private Tilemap _floorTilemap;

    private FloorGrid _floorGrid;
    private SampleReader _reader;
    private List<Node> _nodes;
    //private Node[] _nodesArray;
    private int iterations = 0;

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
                i++;
                j = 1;
                k = 1;
                l = 1;
                m = 1;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetPossibleNodes();
        }
    }

    private void SetPossibleNodes()
    {
        foreach(GridPos gridPos in _floorGrid.GridPositions)
        {
            gridPos.PossibleNodes = _nodes;
        }

        //_nodesArray = _nodes.ToArray();

        StartCoroutine(CheckEntropy());
    }

    private IEnumerator CheckEntropy()
    {
        List<GridPos> tempGrid = new List<GridPos>(_floorGrid.GridPositions);

        tempGrid.RemoveAll(c => c.Collapsed);

        // Orders the uncollapsed positions by their entropy (lowest possible tiles)
        tempGrid.Sort((a, b) => { return a.PossibleNodes.Count - b.PossibleNodes.Count; });

        int numOptions = tempGrid[0].PossibleNodes.Count;
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

        yield return new WaitForSeconds(0.01f);

        CollapseCell(tempGrid);
    }

    private void CollapseCell(List<GridPos> tempGrid)
    {
        // Gets a random tile from the possible ones
        int randomIndex = UnityEngine.Random.Range(0, tempGrid.Count);
        GridPos posToCollapse = tempGrid[randomIndex];

        posToCollapse.Collapsed = true;
        Node selectedNode = posToCollapse.PossibleNodes[UnityEngine.Random.Range(0, posToCollapse.PossibleNodes.Count)];
        posToCollapse.PossibleNodes = new List<Node> { selectedNode };

        _floorTilemap.SetTile((Vector3Int)posToCollapse.CellPosition, selectedNode.Tile);

        UpdateGeneration();
    }

    private void UpdateGeneration()
    {
        List<GridPos> newIterationGrid = new List<GridPos>(_floorGrid.GridPositions);

        int index = 0;

        foreach(GridPos gridPos in newIterationGrid)
        {
            if (_floorGrid.GridPositions[index].Collapsed)
            {
                //newIterationGrid[index] = _floorGrid.GridPositions[index];
                
            }
            else
            {
                List<Node> options = _nodes;

                foreach(Vector2Int offset in Surroundings)
                {
                    if (_floorGrid.TryGetGridPosFromCell(gridPos.CellPosition + offset, out GridPos upPos))
                    {
                        ApplyNeighbourOptions(options, upPos.PossibleNodes);
                    }
                }
                newIterationGrid[index].PossibleNodes = options;
            }
            index++;
        }

        _floorGrid.GridPositions = newIterationGrid;
        iterations++;

        if (iterations < _floorGrid.GridPositions.Count)
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

    public void SetFloorGrid(FloorGrid floorGrid)
    {
        _floorGrid = floorGrid;
    }
}

/*
// Check up node


// Check right node
if (_floorGrid.TryGetGridPosFromCell(gridPos.CellPosition + Surroundings[0], out GridPos upPos2))
{
    List<Node> validOptions = new List<Node>();

    foreach (Node possibleOptions in upPos2.PossibleNodes)
    {
        var valOption = Array.FindIndex(_nodesArray, obj => obj == possibleOptions);
        var valid = _nodesArray[valOption].UpNodes;

        validOptions = validOptions.Concat(valid).ToList();
    }

    ApplyNeighbourOptions(options, validOptions);
}

// Check down node
if (_floorGrid.TryGetGridPosFromCell(gridPos.CellPosition + Surroundings[3], out GridPos upPos3))
{
    List<Node> validOptions = new List<Node>();

    foreach (Node possibleOptions in upPos3.PossibleNodes)
    {
        var valOption = Array.FindIndex(_nodesArray, obj => obj == possibleOptions);
        var valid = _nodesArray[valOption].UpNodes;

        validOptions = validOptions.Concat(valid).ToList();
    }

    ApplyNeighbourOptions(options, validOptions);
}

// Check left node
if (_floorGrid.TryGetGridPosFromCell(gridPos.CellPosition + Surroundings[3], out GridPos upPos4))
{
    ApplyNeighbourOptions(options, upPos4.PossibleNodes);
}
*/