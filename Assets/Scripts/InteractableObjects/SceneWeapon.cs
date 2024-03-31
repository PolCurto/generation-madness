using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneWeapon : InteractableObject
{
    public Weapon Weapon;

    protected override void Interact()
    {
        base.Interact();
        _playerController.SwapWeapon();
    }
}
