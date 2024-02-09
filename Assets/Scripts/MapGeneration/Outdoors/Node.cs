using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Node
{
    private TileBase _tile;
    private int _entropy;

    private List<Node> _rightNodes;
    private List<Node> _lefttNodes;
    private List<Node> _upNodes;
    private List<Node> _downNodes;

    public Node(TileBase tile)
    {
        _tile = tile;
    }





}
