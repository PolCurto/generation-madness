using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempleLevelController : MonoBehaviour
{
    [HideInInspector] public static TempleLevelController Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void OnPlayerEnterRoom(TempleRoom room)
    {
        Debug.Log(room.Position);

        // Tanca portes
        foreach (Connection connection in room.Connections)
        {
            Debug.Log("Connectrion");
            foreach(Bond bond in connection.Bonds)
            {
                bond.DoorController.CloseDoor();
                Debug.Log("Close door");
            }
        }

        // Spawn enemics

        //StartCoroutine(CheckActiveRoomStatus());
    }

    private void OnRoomFinished()
    {

    }

    private IEnumerator CheckActiveRoomStatus()
    {
        while (true)
        {
            yield return null;
        }

        OnRoomFinished();
    }
}
