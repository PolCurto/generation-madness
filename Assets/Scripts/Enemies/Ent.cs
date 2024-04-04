using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ent : HoodSkeleton
{
    protected override void FollowPlayer()
    {
        if (!_playerDetected) return;

        _direction = (_player.position - _rigidbody.position).normalized;           
        Vector2 moveForce = Vector2.MoveTowards(_rigidbody.velocity, _direction.normalized * _maxVelocity, _acceleration * Time.fixedDeltaTime);
        _rigidbody.velocity = moveForce;
    }

    protected override void Attack()
    {
        if (!_isAttacking) return;

        if (_timer - _lastTimeShot > _weaponController.WeaponBase.fireRate)
        {
            _animator.SetTrigger("Shoot");
            _isShooting = true;
            _lastTimeShot = _timer;
        }

        if (_player.position.x < _rigidbody.position.x)
        {
            _spriteRenderer.flipX = true;
        }
        else
        {
            _spriteRenderer.flipX = false;
        }
    }
}
