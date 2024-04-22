using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ent : HoodSkeleton
{
    protected override void FollowPlayer()
    {
        if (!_playerDetected || _path == null) return;

        _direction = (_player.position - _rigidbody.position).normalized;
        Vector2 moveForce = _velocity * Time.deltaTime * _direction;
        _rigidbody.velocity = moveForce;
    }

    protected override void Attack()
    {
        if (!_canAttack) return;

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