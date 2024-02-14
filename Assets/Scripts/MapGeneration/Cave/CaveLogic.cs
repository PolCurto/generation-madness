using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CaveLogic : MonoBehaviour
{
    [SerializeField] private Tilemap _floorTilemap;

    [Header("Cave Logic Parameters")]
    [SerializeField] private int _startPositionArea;

    private bool _startPointIsSet;
    private Vector2Int _startPoint;

    private void SetStartingPoint()
    {
        Vector2Int startPosition = (Vector2Int) _floorTilemap.WorldToCell(Vector3Int.zero);
        Vector2Int offset = Vector2Int.right;

        if (IsValidArea(_startPositionArea, startPosition))
        {
            _startPointIsSet = true;
            _startPoint = Vector2Int.zero;
        }

        int counter = 0;
        int multiplier = 1;

        while (!_startPointIsSet)
        {
            switch (counter)
            {
                case 0: offset = Vector2Int.right * multiplier; break;
                case 1: offset = Vector2Int.down * multiplier; break;
                case 2: offset = Vector2Int.left * multiplier; break;
                case 3: offset = Vector2Int.up * multiplier; break;
            }

            if (IsValidArea(_startPositionArea, startPosition + offset))
            {
                _startPointIsSet = true;
                _startPoint = Vector2Int.zero + offset;
            }

            counter++;

            if (counter == 4)
            {
                counter = 0;
                multiplier++;
            }
        }

        Debug.Log(_startPoint);
    }

    private bool IsValidArea(int offset, Vector2Int position)
    {
        for (int y = position.y - offset; y <= position.y + offset; y++)
        {
            for (int x = position.x - offset; x <= position.x + offset; x++)
            {
                if (!_floorTilemap.HasTile(new Vector3Int(x, y))) return false;
            }
        }
        return true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SetStartingPoint();
        }
    }
}
