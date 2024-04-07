using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponController : MonoBehaviour
{
    [SerializeField] private GameObject _enemy;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private GameObject _bullet;

    [SerializeField] private float _bulletSpawnOffset;

    [SerializeField] private EnemyWeaponBase _weaponBase;

    void Start()
    {
        _spriteRenderer.sprite = _weaponBase.weaponSprite;
    }

    void Update()
    {
        RotateOut();
    }

    private void RotateOut()
    {
        // Rotates the weapon to always point out
        if (transform.position.x - _enemy.transform.position.x < 0)
        {
            transform.localEulerAngles = new Vector3(180, 0, 0);
        }
        else
        {
            transform.localEulerAngles = Vector3.zero;
        }

        if (transform.position.y - _enemy.transform.position.y < 0)
        {
            _spriteRenderer.sortingOrder = 1;
        }
        else
        {
            _spriteRenderer.sortingOrder = -1;
        }
    }


    #region Shooting
    public void Shoot(Vector2 direction)
    {
        float dispersion = _weaponBase.dispersion;

        for (int i = 0; i < _weaponBase.bulletsPerShot; i++)
        {
            direction += new Vector2(Random.Range(-dispersion, dispersion), Random.Range(-dispersion, dispersion));
            Vector3 spawnPosition = transform.position + (transform.right * _bulletSpawnOffset);
            BulletController bullet = Instantiate(_bullet, spawnPosition, Quaternion.identity).GetComponent<BulletController>();
            bullet.SetParameters(direction, _weaponBase.bulletSpeed, _weaponBase.bulletDamage, _weaponBase.bulletDuration, _weaponBase.bulletSprite);
        }
    }

    public EnemyWeaponBase WeaponBase => _weaponBase;
    #endregion
}
