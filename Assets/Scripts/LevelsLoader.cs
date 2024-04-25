using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelsLoader : MonoBehaviour, IDataPersistance
{
    [HideInInspector] public static LevelsLoader Instance;

    public int FloorDepth { get; set; }

    public RunLevelsType RunType { get; set; }

    public enum RunLevelsType
    {
        Default = 0,
        Cave = 1,
        Dungeon = 2,
        Temple = 3
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Loads the corresponding level depending on the run type and the floor depth
    public void LoadNextScene()
    {
        Debug.Log("Run type: " + RunType);
        Debug.Log("Floor depth: " + FloorDepth);

        LoadingScreen.Instance.gameObject.SetActive(true);

        switch (RunType)
        {
            case RunLevelsType.Default:
                switch (FloorDepth)
                {
                    case 0:
                    case 1:
                        LoadScene(1);    // 1 --> Cave
                        break;
                    case 2:
                    case 3:
                        LoadScene(2);    // 2 --> Dungeon
                        break;
                    case 4:
                    case 5:
                        LoadScene(3);    // 3 --> Temple
                        break;
                    default:
                        Debug.LogError("You shouldn't be here");
                        break;
                }
                break;

            case RunLevelsType.Cave:
                LoadScene(1);
                break;

            case RunLevelsType.Dungeon:
                LoadScene(2);
                break;
            case RunLevelsType.Temple:
                LoadScene(3);
                break;
        }
    }

    public void LoadScene(int index)
    {
        StartCoroutine(LoadSceneAsync(index));
    }

    private IEnumerator LoadSceneAsync (int index)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(index);

        while (!operation.isDone)
        {
            yield return null;
        }
    }

    public void LoadData(GameData data)
    {
        FloorDepth = data.floorDepth;
        RunType = (RunLevelsType)data.runType;
    }

    public void SaveData(ref GameData data)
    {
        data.floorDepth = FloorDepth;
        data.runType = (int)RunType;
    }
}
