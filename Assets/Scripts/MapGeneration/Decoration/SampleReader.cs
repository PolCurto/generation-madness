using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SampleReader
{
    private Tilemap _sample;
    private List<Node> _nodes;

    private readonly Vector2Int[] Surroundings = new Vector2Int[]
        {
            new Vector2Int (1, 0),      // Right
            new Vector2Int (0, -1),     // Down
            new Vector2Int (-1, 0),     // Left
            new Vector2Int (0, 1),      // Up
        };

    public SampleReader(Tilemap sample)
    {
        _sample = sample;
        _nodes = new List<Node>();
    }

    public List<Node> GetNodesFromSample()
    {
        TileBase currentTile = null;
        Node newNode = null;

        // Create the node for each tile
        foreach (Vector3Int position in _sample.cellBounds.allPositionsWithin)
        {
            if (_sample.HasTile(position))
            {
                currentTile = _sample.GetTile(position);

                if (!NodeHasTile(currentTile, _nodes))
                {
                    newNode = new Node(currentTile);
                    _nodes.Add(newNode);
                }
            }   
        }
        Debug.Log("Nodes count: " + _nodes.Count);

        // Search the neighbours each node has
        foreach (Vector3Int position in _sample.cellBounds.allPositionsWithin)
        {
            if (_sample.HasTile(position))
            {
                currentTile = _sample.GetTile(position);
                newNode = GetNodeFromTile(currentTile);
                SetNodeNeighbours(newNode, position);
            }
        }

        return _nodes;
    }

    private void SetNodeNeighbours(Node currentNode, Vector3Int position)
    {
        int counter = 0;

        foreach (Vector2Int offset in Surroundings)
        {
            Vector3Int neighbourPos = position + (Vector3Int)offset;
            if (_sample.HasTile(neighbourPos))
            {
                TileBase neighbourTile = _sample.GetTile(neighbourPos);
                Node neighbourNode = GetNodeFromTile(neighbourTile);
                switch(counter)
                {
                    case 0:
                        if (!currentNode.RightNodes.Contains(neighbourNode))
                        {
                            currentNode.RightNodes.Add(neighbourNode);
                        }
                        break;
                    case 1:
                        if (!currentNode.DownNodes.Contains(neighbourNode))
                        {
                            currentNode.DownNodes.Add(neighbourNode);
                        }
                        break;
                    case 2:
                        if (!currentNode.LeftNodes.Contains(neighbourNode))
                        {
                            currentNode.LeftNodes.Add(neighbourNode);
                        }
                        break;
                    case 3:
                        if (!currentNode.UpNodes.Contains(neighbourNode))
                        {
                            currentNode.UpNodes.Add(neighbourNode);
                        }
                        break;
                }
            }
            counter++;
        }
    }

    private bool NodeHasTile(TileBase tile, List<Node> nodes)
    {
        foreach (Node node in nodes)
        {
            if (node.Tile == tile) return true;
        }
        return false;
    }

    private Node GetNodeFromTile(TileBase tile)
    {
        foreach(Node node in _nodes)
        {
            if (node.Tile == tile) return node;
        }

        Debug.Log("No node has that tile");
        return null;
    }

    private void GetDirectionalNeighbour()
    {

    }

}
