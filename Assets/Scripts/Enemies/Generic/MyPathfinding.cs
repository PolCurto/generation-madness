using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPathfinding : MonoBehaviour
{
    /*
    private const int STRAIGHT_MOVE_COST = 10;
    private const int DIAGONAL_MOVE_COST = 14;

    public static MyPathfinding Instance;

    [SerializeField] private FloorGrid _floorGrid;
    private List<GridPos> _openList;
    private List<GridPos> _closedList;

    private void Awake()
    {
        Instance = this;    
    }

    public void SetFloorGrid(FloorGrid floorGrid)
    {
        _floorGrid = floorGrid;
    }

    public List<Vector2> FindVectorPath(Vector2 startPosition, Vector2 endPosition)
    {
        List<Vector2> vectorPath = new List<Vector2>();

        Vector2Int roundStartPos = new Vector2Int(Mathf.FloorToInt(startPosition.x),Mathf.FloorToInt(startPosition.y));
        //Debug.Log("Rigidbody pos:" + startPosition);
        //Debug.Log("Start pos round: " + roundStartPos);

        Vector2Int roundEndPos = new Vector2Int(Mathf.FloorToInt(endPosition.x), Mathf.FloorToInt(endPosition.y));
        //Debug.Log("Player pos:" + endPosition);
        //Debug.Log("End pos round: " + roundEndPos);

        List<GridPos> gridPath = FindPath(roundStartPos, roundEndPos);
        if (gridPath == null) return null;
        
        foreach(GridPos gridPos in gridPath)
        {
            Vector2 position = gridPos.WorldPosition;
            position.x += 0.5f;
            position.y += 0.5f;
            vectorPath.Add(position);
        }
        return vectorPath;
    }

    public List<GridPos> FindPath(Vector2Int startPosition, Vector2Int endPosition)
    {
        GridPos startPos = _floorGrid.GetGridPosFromWorld(startPosition);
        GridPos endPos = _floorGrid.GetGridPosFromWorld(endPosition);

        if (startPos == null || endPos == null) return null;

        _openList = new List<GridPos> { startPos };
        _closedList = new List<GridPos>();

        foreach(GridPos gridPos in _floorGrid.GridPositions)
        {
            gridPos.GCost = int.MaxValue;
            gridPos.CalculateFCost();
            gridPos.CameFrom = null;
        }

        startPos.GCost = 0;
        startPos.HCost = ManhattanDistance(startPos, endPos);
        startPos.CalculateFCost();

        int iterator = 0;

        while (_openList.Count > 0)
        {
            GridPos currentPos = GetLowestFCostPos(_openList);
            if (currentPos == endPos)
            {
                // End
                return GetUsedPath(endPos);
            }

            _openList.Remove(currentPos);
            _closedList.Add(currentPos);

            foreach (GridPos neighbourPos in currentPos.Neighbours)
            {
                // Avoids corners
                if (_closedList.Contains(neighbourPos) || (neighbourPos.IsCorner() && (neighbourPos != startPos && neighbourPos != endPos))) continue;

                int tempGCost = currentPos.GCost + ManhattanDistance(currentPos, neighbourPos);
                if (tempGCost < neighbourPos.GCost) 
                {
                    neighbourPos.CameFrom = currentPos;
                    neighbourPos.GCost = tempGCost;
                    neighbourPos.HCost = ManhattanDistance(neighbourPos, endPos);
                    neighbourPos.CalculateFCost();

                    if (!_openList.Contains(neighbourPos))
                    {
                        _openList.Add(neighbourPos);
                    }
                }
            }
            iterator++;

            if (iterator > 10000)
            {
                Debug.Log("Esto peta");
                return null;
            }
        }

        Debug.Log("A path could not be found");
        return null;
    }

    private List<GridPos> GetUsedPath(GridPos endPos)
    {
        List<GridPos> path = new List<GridPos>();
        path.Add(endPos);
        GridPos currentPos = endPos;
        while(currentPos.CameFrom != null)
        {
            path.Add(currentPos.CameFrom);
            currentPos = currentPos.CameFrom;
        }

        path.Reverse();
        return path;
    }

    private int ManhattanDistance(GridPos a, GridPos b)
    {
        int distX = Mathf.Abs(a.CellPosition.x - b.CellPosition.x);
        int distY = Mathf.Abs(a.CellPosition.y - b.CellPosition.y);
        int rest = Mathf.Abs(distX - distY);

        return DIAGONAL_MOVE_COST * Mathf.Min(distX, distY) + STRAIGHT_MOVE_COST * rest;
    }

    private GridPos GetLowestFCostPos(List<GridPos> gridPosList)
    {
        GridPos lowestFCostPos = gridPosList[0];
        for (int i = 1; i < gridPosList.Count; i++)
        {
            if (gridPosList[i].FCost < lowestFCostPos.FCost)
            {
                lowestFCostPos = gridPosList[i];
            }
        }
        return lowestFCostPos;
    }
    */
}
 