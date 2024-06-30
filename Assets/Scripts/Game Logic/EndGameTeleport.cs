using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameTeleport : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            AudioManager.Instance.SetMusicVolume(0.2f);

            DataPersistanceManager.Instance.NewGame();
            UIController.Instance.OnGameFinished();
        }
    }
}
