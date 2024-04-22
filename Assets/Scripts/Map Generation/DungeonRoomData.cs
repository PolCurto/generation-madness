using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Room", menuName = "Dungeon Room")]
public class DungeonRoomData : ScriptableObject
{
    public GameObject roomTilemap;

    public GameObject[] enemyPool;
    public int[] enemyTypeLimits;
    public Vector2[] enemyPositions;
}
