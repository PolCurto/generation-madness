using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CaveDecoration : MonoBehaviour
{
    [SerializeField] private GameObject _sample;
    [SerializeField] private Tilemap _floorTilemap;

    private FloorGrid _floorGrid;
    private SampleReader _reader;
    private List<Node> _nodes;

    private void Start()
    {
        _reader = new SampleReader(_sample.GetComponentInChildren<Tilemap>());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Alpha1))
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

        if (Input.GetKeyDown(KeyCode.Alpha8))
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

        CheckEntropy();
    }

    private void CheckEntropy()
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

        CollapseCell(tempGrid);
    }

    private void CollapseCell(List<GridPos> tempGrid)
    {
        // Gets a random tile from the possible ones
        int randomIndex = Random.Range(0, tempGrid.Count);
        GridPos posToCollapse = tempGrid[randomIndex];

        posToCollapse.Collapsed = true;
        Node selectedNode = posToCollapse.PossibleNodes[Random.Range(0, posToCollapse.PossibleNodes.Count)];
        posToCollapse.PossibleNodes = new List<Node> { selectedNode };

        _floorTilemap.SetTile((Vector3Int)posToCollapse.CellPosition, selectedNode.Tile);

        UpdateGeneration();
    }

    private void UpdateGeneration()
    {

    }

    public void SetFloorGrid(FloorGrid floorGrid)
    {
        _floorGrid = floorGrid;
    }
}
