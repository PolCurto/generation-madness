using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneWeapon : InteractableObject
{
    [SerializeField] private WeaponBase _weapon;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _spriteRenderer.sprite = _weapon.weaponSprite;
    }

    protected override void Interact()
    {
        Debug.Log("Interact");
        base.Interact();
        _playerController.AddWeapon(_weapon);
    }
}