using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bandit : HoodSkeleton
{
    protected override void Shoot()
    {
        Vector2 direction = _player.position - (_rigidbody.position + _rigidbody.velocity);
        _weaponController.Shoot(direction);
    }
}
