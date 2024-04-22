using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrookShroom : HoodSkeleton
{
    [Header("Crook Shroom Parameters")]
    [SerializeField] float _velocityMultiplier;
    protected override void FollowPlayer()
    {
        if (!_playerDetected || _canAttack || _path == null || _isShooting) return;

        _direction = (_player.position - _rigidbody.position).normalized;
        Vector2 moveForce = _velocity * Time.deltaTime * _direction;
        _rigidbody.velocity = moveForce * _velocityMultiplier;
    }
}
