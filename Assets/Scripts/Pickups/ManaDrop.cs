using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mana : Pickup
{
    [SerializeField] private int _mana;

    protected override void OnPickUp(PlayerController player)
    {
        player.RestoreAmmo(_mana);
        AudioManager.Instance.PlaySFX("Pick Ammo", 1);

        base.OnPickUp(player);
    }
}
