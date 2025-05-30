using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject _loadingScreen;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    private void Start()
    {
        if (AudioManager.Instance.IsPlayingMusic()) AudioManager.Instance.StopMusic();
    }

    public void StartNewGame(int type)
    {
        Debug.Log("Start new game");
        DataPersistanceManager.Instance.NewGame();
        LevelsLoader.Instance.RunType = (LevelsLoader.RunLevelsType)type;

        DataPersistanceManager.Instance.SaveGame();
        LevelsLoader.Instance.LoadNextScene();
    }

    public void ContinueGame()
    {
        if (!DataPersistanceManager.Instance.LoadGame())
        {
            Debug.LogWarning("There is no data to continue from");
            return;
        }
        LevelsLoader.Instance.LoadNextScene();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
