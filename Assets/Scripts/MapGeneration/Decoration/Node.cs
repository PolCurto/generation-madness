using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Node
{
    public TileBase Tile { get; set; }
    private int _entropy;

    public List<Node> RightNodes { get; set; }
    public List<Node> LeftNodes { get; set; }
    public List<Node> UpNodes { get; set; }
    public List<Node> DownNodes { get; set; }

    public Node(TileBase tile)
    {
        Tile = tile;
        RightNodes = new List<Node>();
        LeftNodes = new List<Node>();
        UpNodes = new List<Node>();
        DownNodes = new List<Node>();
    }
}
