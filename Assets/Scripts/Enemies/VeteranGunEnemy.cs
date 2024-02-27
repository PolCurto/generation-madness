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
        Vector2 moveForce = Vector2.MoveTowards(_rigidbody.velocity, direction.normalized * _maxVelocity, _acceleration * Time.fixedDeltaTime);
        _rigidbody.velocity = moveForce;
    }
}