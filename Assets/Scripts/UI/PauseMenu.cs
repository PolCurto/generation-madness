using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [HideInInspector] public static UIController Instance;

    [SerializeField] private GameObject _pauseMenu;

    private void Awake()
    {
        Instance = this;
    }

    public void TogglePauseMenu()
    {
        if (_pauseMenu.activeSelf)
        {
            Time.timeScale = 1f;
            _pauseMenu.SetActive(false);
        }
        else
        {
            _pauseMenu.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void GoMainMenu()
    {
        Time.timeScale = 1f;
        LevelsLoader.Instance.LoadScene(0);
    }
}
