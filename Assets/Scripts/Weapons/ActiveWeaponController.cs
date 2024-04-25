using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveWeaponController : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private GameObject _bullet;

    [SerializeField] private float _bulletSpawnOffset;

    private Animator _animator;
    private Weapon _weapon;
    private float _timer;
    private float _lastTimeShot;
    private PlayerController _playerController;

    public bool IsReloading { get; set; }

    void Awake()
    {
        _playerController = _player.GetComponent<PlayerController>();
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        RotateOut();

        _timer += Time.deltaTime;
    }

    private void RotateOut()
    {
        if (transform.position.x - _player.transform.position.x < 0)
        {
            transform.localEulerAngles = new Vector3(180, 0, 0);
        }
        else
        {
            transform.localEulerAngles = Vector3.zero;
        }

        if (transform.position.y - _player.transform.position.y < 0)
        {
            _spriteRenderer.sortingOrder = 1;
        }
        else
        {
            _spriteRenderer.sortingOrder = -1;
        }
    }

    public void SwapWeapon(Weapon weapon)
    {
        _weapon = weapon;
        _spriteRenderer.sprite = _weapon.WeaponBase.weaponSprite;
        _animator.Play("Pistol_Idle");
        _animator.runtimeAnimatorController  = _weapon.WeaponBase.overrideController;

       // UIController.Instance.UpdateWeaponAtIndex

        CameraController.Instance.ResetOffset();

        CameraController.Instance.MaxOffset *= _weapon.WeaponBase.cameraOffsetMultiplier;
        CameraController.Instance.MaxOffset *= _weapon.WeaponBase.cameraOffsetMultiplier;
    }

    #region Shooting
    public void Shoot(Vector2 direction)
    {
        if (IsReloading || _timer - _lastTimeShot < _weapon.WeaponBase.fireRate * _playerController.AttackSpeed) return;

        if (_weapon.ClipBullets > 0)
        {
            float dispersion = _weapon.WeaponBase.dispersion;

            for (int i = 0; i < _weapon.WeaponBase.bulletsPerShot; i++)
            {
                direction += new Vector2(Random.Range(-dispersion, dispersion), Random.Range(-dispersion, dispersion));
                Vector3 spawnPosition = transform.position + (transform.right * _bulletSpawnOffset);
                BulletController bullet = Instantiate(_bullet, spawnPosition, Quaternion.identity).GetComponent<BulletController>();
                SetBulletParameters(bullet, direction);
            }

            _weapon.Shoot();

            _lastTimeShot = _timer;

            UIController.Instance.UpdateAmmoAtIndex(_playerController.ActiveWeaponIndex, _weapon.ClipBullets, _weapon.TotalBullets);
        }
        else
        {
            StartReload();
        }
    }

    public void StartReload()
    {
        if (_weapon.ClipBullets == _weapon.WeaponBase.clipSize || _weapon.TotalBullets == 0) return;    // Only reload if there are bullets missing
        IsReloading = true;
        _animator.SetTrigger("Reload");
    }

    public void ReloadWeapon()
    {
        _weapon.Reload();
        UIController.Instance.UpdateAmmoAtIndex(_playerController.ActiveWeaponIndex, _weapon.ClipBullets, _weapon.TotalBullets);

    }

    private void SetBulletParameters(BulletController bullet, Vector2 direction)
    {
        float speed = _weapon.WeaponBase.bulletSpeed * _playerController.BulletSpeed;
        int damage = Mathf.RoundToInt(_weapon.WeaponBase.bulletDamage * _playerController.DamageMultiplier);

        bullet.SetParameters(direction, speed, damage, _weapon.WeaponBase.bulletDuration, _weapon.WeaponBase.bulletSprite);
    }
    #endregion

    public void RestoreAmmo(int amount)
    {
        _weapon.RestoreAmmo(amount);
        UIController.Instance.UpdateAmmoAtIndex(_playerController.ActiveWeaponIndex, _weapon.ClipBullets, _weapon.TotalBullets);
    }

    public Weapon Weapon => _weapon;
}
