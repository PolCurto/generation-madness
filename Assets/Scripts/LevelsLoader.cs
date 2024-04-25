using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelsLoader : MonoBehaviour, IDataPersistance
{
    [HideInInspector] public static LevelsLoader Instance;

    private int _floorDepth;

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
        _floorDepth = data.floorDepth;
    }

    public void SaveData(ref GameData data)
    {
        data.floorDepth = _floorDepth;
    }
}
