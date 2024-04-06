using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medkit : Pickup
{
    [SerializeField] private int _health;

    protected override void OnPickUp(PlayerController player)
    {
        player.Heal(_health);

        base.OnPickUp(player);
    }
}
