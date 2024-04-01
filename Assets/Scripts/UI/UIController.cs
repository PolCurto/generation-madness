using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [HideInInspector] public static UIController Instance;

    

    private void Awake()
    {
        Instance = this;
    }

    #region Health
    [Header("Health")]
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

    #region Weapons
    [Header("Weapons")]
    [SerializeField] private CanvasGroup[] _weapons;
    [SerializeField] private TextMeshProUGUI[] _clipBullets;
    [SerializeField] private Image[] _weaponsSprite;

    public void HighlightWeaponAtIndex(int index)
    {
        for (int i = 0; i < _weapons.Length; i++)
        {
            if (i == index) _weapons[i].alpha = 1;
            else _weapons[i].alpha = 0.5f;
        }
    }

    public void UpdateAmmoAtIndex(int index, int clipBullets)
    {
        _clipBullets[index].text = clipBullets.ToString();
        // Falta la barra
    }

    public void UpdateWeaponAtIndex(int index, Weapon weapon)
    {
        _weaponsSprite[index].sprite = weapon.WeaponBase.weaponSprite;
        _clipBullets[index].text = weapon.ClipBullets.ToString();
        // Falta la barra
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
