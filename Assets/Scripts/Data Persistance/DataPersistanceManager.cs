using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class DataPersistanceManager : MonoBehaviour
{
    public static DataPersistanceManager Instance { get; private set; }

    [Header("File Storage Config")]
    [SerializeField] private string filename;

    private GameData _gameData;
    private List<IDataPersistance> _dataPersistanceObjects;
    private FileDataHandler _dataHandler;

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

    private void Start()
    {
        _dataPersistanceObjects = new List<IDataPersistance>();
        _dataPersistanceObjects = FindAllDataPersistanceObjects();
        _dataHandler = new FileDataHandler(Application.persistentDataPath, filename);

        LoadGame();
    }

    public void NewGame()
    {
        _gameData = new GameData();
        LevelsLoader.Instance.FloorDepth = 0;
    }

    public bool LoadGame()
    {
        bool dataExists = true;

        _gameData = _dataHandler.Load();

        if (_gameData == null)
        {
            dataExists = false;
            NewGame();
        }

        foreach(IDataPersistance dataPersistanceObj in _dataPersistanceObjects)
        {
            dataPersistanceObj.LoadData(_gameData);
        }

        return dataExists;
    }

    public void SaveGame()
    {
        foreach (IDataPersistance dataPersistanceObj in _dataPersistanceObjects)
        {
            dataPersistanceObj.SaveData(ref _gameData);
        }

        LevelsLoader.Instance.SaveData(ref _gameData);

        _dataHandler.Save(_gameData);
    }

    private List<IDataPersistance> FindAllDataPersistanceObjects()
    {
        IEnumerable<IDataPersistance> dataPersistanceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistance>();

        return new List<IDataPersistance>(dataPersistanceObjects);
    }
}
