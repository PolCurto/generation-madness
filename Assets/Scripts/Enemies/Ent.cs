using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ent : HoodSkeleton
{
    protected override void FollowPlayer()
    {
        if (!_playerDetected || _path == null) return;

        if (_currentWaypoint >= _path.vectorPath.Count)
        {
            _reachedEndOfPath = true;
            return;
        }
        else
        {
            _reachedEndOfPath = false;
        }

        _direction = ((Vector2)_path.vectorPath[_currentWaypoint] - _rigidbody.position).normalized;
        Vector2 moveForce = Vector2.MoveTowards(_rigidbody.velocity, _direction * _maxVelocity, _velocity * Time.fixedDeltaTime);
        _rigidbody.velocity = moveForce;

        float distance = Vector2.Distance(_rigidbody.position, _path.vectorPath[_currentWaypoint]);
        if (distance < _nextWaypointDistance)
        {
            _currentWaypoint += 1;
        }
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