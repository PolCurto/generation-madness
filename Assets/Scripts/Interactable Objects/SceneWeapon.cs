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
        SetBaseWeapon(_weaponBase);
    }

    protected override void Interact()
    {
        base.Interact();
        _playerController.AddWeapon(_weapon);
        Destroy(gameObject);
    }

    public void SetWeapon(Weapon weapon)
    {
        _weaponBase = weapon.WeaponBase;
        _weapon = weapon;
        _spriteRenderer.sprite = _weaponBase.weaponSprite;
    }

    public void SetBaseWeapon(WeaponBase weaponBase)
    {
        _weaponBase = weaponBase;
        _weapon = new Weapon(weaponBase);
        _spriteRenderer.sprite = _weaponBase.weaponSprite;
    }
}