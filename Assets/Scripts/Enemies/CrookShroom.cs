using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrookShroom : HoodSkeleton
{
    [Header("Crook Shroom Parameters")]
    [SerializeField] float _velocityMultiplier;
    protected override void FollowPlayer()
    {
        if (!_playerDetected || _isAttacking || _isShooting) return;

        _direction = (_player.position - _rigidbody.position).normalized;
        Vector2 moveForce = Vector2.MoveTowards(_rigidbody.velocity, _direction.normalized * _maxVelocity, _acceleration * Time.fixedDeltaTime);
        _rigidbody.velocity = moveForce * _velocityMultiplier;
    }
}
