using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [HideInInspector] public static UIController Instance;

    

    private void Awake()
    {
        Instance = this;
    }

    #region HUD
    [Header("HUD")]
    [SerializeField] private Image[] _hearts;
    [SerializeField] private Sprite _fullHeart;
    [SerializeField] private Sprite _emptyHeart;

    public void UpdateLife(int maxLife, int currentLife)
    {
        for (int i = 0; i < maxLife; i++)
        {
            if (!_hearts[i].gameObject.activeSelf)
            {
                _hearts[i].gameObject.SetActive(true);
            }

            if (i < currentLife)
            {
                _hearts[i].sprite = _fullHeart;
            } 
            else
            {
                _hearts[i].sprite = _emptyHeart;
            }
        }
    }

    #endregion

    #region Pause Menu
    [Header("Pause Menu")]
    [SerializeField] private GameObject _pauseMenu;
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
    #endregion

    
}
