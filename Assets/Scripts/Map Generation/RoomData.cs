using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Room", menuName = "Room")]
public class RoomData : ScriptableObject
{
    public GameObject roomTilemap;

    public GameObject[] items;
    public Vector2[] itemsPositions;

    public GameObject[] enemies;
    public Vector2[] enemyPositions;
}
