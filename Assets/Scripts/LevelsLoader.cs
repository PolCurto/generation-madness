using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelsLoader : MonoBehaviour, IDataPersistance
{
    [HideInInspector] public static LevelsLoader Instance;

    public int FloorDepth { get; set; }

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

    public void LoadScene(int index)
    {
        StartCoroutine(LoadSceneAsync(index));
    }

    private IEnumerator LoadSceneAsync (int index)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(index);

        while (!operation.isDone)
        {
            Debug.Log(operation.progress);
            yield return null;
        }
    }

    public void LoadData(GameData data)
    {
        FloorDepth = data.floorDepth;
    }

    public void SaveData(ref GameData data)
    {
        data.floorDepth = FloorDepth;
    }
}
