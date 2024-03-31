using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneWeapon : InteractableObject
{
    [SerializeField] private Weapon _weapon;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _spriteRenderer.sprite = _weapon.itemSprite;
    }

    protected override void Interact()
    {
        Debug.Log("Interact");
        base.Interact();
        _playerController.AddWeapon(_weapon);
    }
}