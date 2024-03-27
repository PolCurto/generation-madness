using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject _loadingScreen;

    public void GenerateCaveLevel()
    {
        LoadingScreen.Instance.gameObject.SetActive(true);
        gameObject.SetActive(false);
        LevelsLoader.Instance.LoadScene(1);
    }

    public void GenerateDungeonLevel()
    {
        LoadingScreen.Instance.gameObject.SetActive(true);
        gameObject.SetActive(false);
        LevelsLoader.Instance.LoadScene(2);
    }

    public void GenerateTempleLevel()
    {
        LoadingScreen.Instance.gameObject.SetActive(true);
        gameObject.SetActive(false);
        LevelsLoader.Instance.LoadScene(3);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
