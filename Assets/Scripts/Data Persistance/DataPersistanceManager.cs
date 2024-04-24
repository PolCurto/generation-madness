using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataPersistanceManager : MonoBehaviour
{
    public static DataPersistanceManager Instance { get; private set; }

    private GameData _gameData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }          
        else
        {
            Destroy(gameObject);
        }       
    }

    public void NewGame()
    {

    }

    public void LoadGame()
    {

    }

    public void SaveGame()
    {

    }
}
