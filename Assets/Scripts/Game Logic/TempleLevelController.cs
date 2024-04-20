using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempleLevelController : FloorLogicController
{
    public override void OnPlayerEnterRoom(TempleRoom room)
    {
        if (room.Completed) return;

        // Tanca portes
        foreach (Connection connection in room.Connections)
        {
            foreach(Bond bond in connection.Bonds)
            {
                bond.DoorController.CloseDoor();
            }
        }

        List<GameObject> enemies = new List<GameObject>();

        // Spawn enemics
        foreach (KeyValuePair<Vector2, GameObject> enemyData in room.Enemies)
        {
            Vector2 enemyPos = enemyData.Key + room.Position;
            enemies.Add(Instantiate(enemyData.Value, enemyPos, Quaternion.identity));
        }

        Debug.Log("Enter room");

        StartCoroutine(CheckActiveRoomStatus(room, enemies));
    }

    private void OnRoomFinished(TempleRoom room)
    {
        Debug.Log("Finished room");

        foreach (Connection connection in room.Connections)
        {
            foreach (Bond bond in connection.Bonds)
            {
                bond.DoorController.OpenDoor();
            }
        }

        room.Completed = true;
    }

    private IEnumerator CheckActiveRoomStatus(TempleRoom room, List<GameObject> enemies)
    {
        bool enemiesAlive = true;
        int aliveCounter;

        while (enemiesAlive)
        {
            aliveCounter = 0;

            foreach (GameObject enemy in enemies)
            {
                if (enemy.activeSelf)
                {
                    aliveCounter++;
                }
            }

            yield return null;

            if (aliveCounter == 0)
            {
                enemiesAlive = false;
            }
        }

        OnRoomFinished(room);
    }
}
