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
        Vector2 moveForce = Vector2.MoveTowards(_rigidbody.velocity, _direction * (_maxVelocity * _velocityMultiplier), _velocity * Time.fixedDeltaTime);
        _rigidbody.velocity = moveForce;

        float distance = Vector2.Distance(_rigidbody.position, _path.vectorPath[_currentWaypoint]);
        if (distance < _nextWaypointDistance)
        {
            _currentWaypoint += 1;
        }
    }
}
