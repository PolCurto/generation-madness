using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medkit : Pickup
{
    [SerializeField] private int _health;

    protected override void OnPickUp(PlayerController player)
    {
        player.Heal(_health);
        AudioManager.Instance.PlaySFX("Heal", 0.2f);

        base.OnPickUp(player);
    }
}
