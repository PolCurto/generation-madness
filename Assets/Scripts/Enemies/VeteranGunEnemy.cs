using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VeteranGunEnemy : Enemy
{
    protected override void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        FollowPlayer();
    }

    private void FollowPlayer()
    {
        if (!_playerDetected) return;

        Vector2 direction = (Vector2)_player.position - _rigidbody.position;
        Vector2 moveForce = direction.normalized * _velocity;
        _rigidbody.velocity = moveForce;
    }
}