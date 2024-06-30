using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextFloorTeleport : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            AudioManager.Instance.SetMusicVolume(0.2f);

            LevelsLoader.Instance.FloorDepth += 1;
            DataPersistanceManager.Instance.SaveGame();
            UIController.Instance.OnLevelCompleted();
        }
    }
}
