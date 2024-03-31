using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveWeaponController : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private GameObject _bullet;

    [SerializeField] private float _bulletSpawnOffset;

    private Weapon _weapon;
    private float _timer;
    private float _lastTimeShot;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
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
    }

    public void SwapWeapon(Weapon weapon)
    {
        _weapon = weapon;
        _spriteRenderer.sprite = _weapon.itemSprite;

        CameraController.Instance.ResetOffset();

        CameraController.Instance.MaxOffset *= _weapon.cameraOffsetMultiplier;
        CameraController.Instance.MaxOffset *= _weapon.cameraOffsetMultiplier;
    }

    #region Shooting
    public void Shoot(Vector2 direction)
    {
        if (_timer - _lastTimeShot < _weapon.fireRate) return;

        for (int i = 0; i < _weapon.bulletsPerShot; i++)
        {
            Vector3 spawnPosition = transform.position + (transform.right * _bulletSpawnOffset);
            BulletController bullet = Instantiate(_bullet, spawnPosition, Quaternion.identity).GetComponent<BulletController>();
            bullet.SetParameters(direction, _weapon.bulletSpeed, _weapon.bulletDamage, _weapon.bulletDuration, _weapon.bulletSprite);
        }

        _lastTimeShot = _timer;
    }
    #endregion

    public Weapon Weapon => _weapon;
}
