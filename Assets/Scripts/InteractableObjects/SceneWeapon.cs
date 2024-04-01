using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneWeapon : InteractableObject
{
    [SerializeField] private WeaponBase _weaponBase;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private Weapon _weapon;

    private void Awake()
    {
        if (_weaponBase != null)
        {
            _spriteRenderer.sprite = _weaponBase.weaponSprite;
            _weapon = new Weapon(_weaponBase);
        }
    }

    protected override void Interact()
    {
        base.Interact();
        _playerController.AddWeapon(_weapon);
        Destroy(gameObject);
    }

    public void SetWeaponBase(WeaponBase weaponBase)
    {
        _weaponBase = weaponBase;
        _weapon = new Weapon(weaponBase);
        _spriteRenderer.sprite = _weaponBase.weaponSprite;
    }

    public void SetWeapon(Weapon weapon)
    {
        _weaponBase = weapon.WeaponBase;
        _weapon = weapon;
        _spriteRenderer.sprite = _weaponBase.weaponSprite;
    }
}