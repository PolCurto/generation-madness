using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneWeapon : InteractableObject
{
    [SerializeField] private WeaponBase _weaponBase;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private SpriteRenderer _minimapIcon;
    [SerializeField] private SpriteRenderer _border;

    private Weapon _weapon;
    private Animator _animator;

    private void Awake()
    {
        SetBaseWeapon(_weaponBase);
        _animator = GetComponent<Animator>();   
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        if (collision.CompareTag("Player"))
        {
            _border.enabled = true;
            _animator.Play("Interactable");
        }
            
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);

        if (collision.CompareTag("Player"))
        {
            _border.enabled = false;
            _animator.SetTrigger("Stop");
        }
            
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
        _minimapIcon.sprite = _weaponBase.weaponSprite;
    }

    public void SetBaseWeapon(WeaponBase weaponBase)
    {
        _weaponBase = weaponBase;
        _weapon = new Weapon(weaponBase);
        _spriteRenderer.sprite = _weaponBase.weaponSprite;
        _minimapIcon.sprite = _weaponBase.weaponSprite;
    }
}