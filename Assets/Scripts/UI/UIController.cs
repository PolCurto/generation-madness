using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    [SerializeField] private Slider[] _ammoSliders;
    [SerializeField] private Image[] _ammoBars;
    [SerializeField] private float _hiddenWeaponsAlpha;

    public void HighlightWeaponAtIndex(int index, int weaponsCount)
    {
        for (int i = 0; i < weaponsCount; i++)
        {
            if (i == index) _weapons[i].alpha = 1;
            else _weapons[i].alpha = _hiddenWeaponsAlpha;
        }
    }

    public void UpdateAmmoAtIndex(int index, int clipBullets, int totalBullets)
    {
        _clipBullets[index].text = clipBullets.ToString();
        _ammoSliders[index].value = totalBullets;
    }

    public void UpdateWeaponAtIndex(int index, Weapon weapon)
    {
        _weaponsSprite[index].sprite = weapon.WeaponBase.weaponSprite;
        _clipBullets[index].text = weapon.ClipBullets.ToString();

        _ammoSliders[index].maxValue = weapon.WeaponBase.maxBullets;
        _ammoSliders[index].value = weapon.TotalBullets;

        _ammoBars[index].color = weapon.WeaponBase.color;
    }

    public void ShowSecondWeapon()
    {
        _weapons[1].alpha = _hiddenWeaponsAlpha;
    }

    #endregion

    #region Pause Menu
    [Header("Pause Menu")]
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private TextMeshProUGUI _velocityValue;
    [SerializeField] private TextMeshProUGUI _damageValue;
    [SerializeField] private TextMeshProUGUI _attackSpeedValue;
    [SerializeField] private TextMeshProUGUI _reloadSpeedValue;
    [SerializeField] private TextMeshProUGUI _bulletSpeedValue;

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

    public void UpdateStats(float velocity, float damage, float atckSpd, float reload, float bulletSpd)
    {
        _velocityValue.text = velocity.ToString("#.00");
        _damageValue.text = damage.ToString("#.00");
        _attackSpeedValue.text = atckSpd.ToString("#.00");
        _reloadSpeedValue.text = reload.ToString("#.00");
        _bulletSpeedValue.text = bulletSpd.ToString("#.00");
    }

    public void GoMainMenu()
    {
        Time.timeScale = 1f;
        LevelsLoader.Instance.LoadScene(0);
    }
    #endregion

    #region Level Complete Screen
    [Header("Level Complete Screen")]
    [SerializeField] private GameObject _levelCompleteScreen;

    public void OnLevelCompleted()
    {
        _levelCompleteScreen.SetActive(true);
        Time.timeScale = 0;
    }

    public void LoadNextFloor()
    {
        Time.timeScale = 1f;
        LevelsLoader.Instance.LoadNextScene();
    }
    #endregion

    #region Game Finished Screen
    [Header("Level Complete Screen")]
    [SerializeField] private GameObject _gameFinishedScreen;
    public void OnGameFinished()
    {
        _gameFinishedScreen.SetActive(true);
        Time.timeScale = 0;
    }
    #endregion

    #region Death Screen
    [Header("Death Screen")]
    [SerializeField] private GameObject _deathScreen;

    public void OnDeath()
    {
        _deathScreen.SetActive(true);
        Time.timeScale = 0;
    }
    #endregion
}
