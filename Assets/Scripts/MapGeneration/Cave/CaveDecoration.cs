using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CaveDecoration : MonoBehaviour
{
    [SerializeField] private GameObject _sample;

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
    }
}
