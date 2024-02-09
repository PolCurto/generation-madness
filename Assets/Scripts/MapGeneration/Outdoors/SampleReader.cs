using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SampleReader : MonoBehaviour
{
    [SerializeField] private Tilemap _sample;

    private void Start()
    {
        GetNodesFromSample();
    }

    private void GetNodesFromSample()
    {
        /*
        foreach (Vector3Int position in _sample.cellBounds.allPositionsWithin)
        {
            Debug.Log(position);
        }
        */
    }
}
