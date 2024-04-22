using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class HoodSkeleton : Enemy
{
    [Header("Basic Enemy Parameters")]
    [SerializeField] protected EnemyWeaponController _weaponController;
    [SerializeField] protected Transform _referencePoint;

    protected float _lastTimeShot;
    protected bool _isShooting;

    protected override void Update()
    {
        base.Update();
        MoveWeapon();
        Attack();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        FollowPlayer();
    }

    protected virtual void FollowPlayer()
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
        Vector2 moveForce = _maxVelocity * Time.deltaTime * _direction;
        _rigidbody.velocity = moveForce;

        float distance = Vector2.Distance(_rigidbody.position, _path.vectorPath[_currentWaypoint]);
        if (distance < _nextWaypointDistance)
        {
            _currentWaypoint += 1;
        }

        /*
        Vector2 direction = (Vector2)_player.position - _rigidbody.position;
        Vector2 moveForce = Vector2.MoveTowards(_rigidbody.velocity, direction.normalized * _maxVelocity, _acceleration * Time.fixedDeltaTime);
        _rigidbody.velocity = moveForce;
        

        if (IsInGoalPosition())
        {
            _direction = _pathToTake[1];
            _pathToTake.RemoveAt(0);
        }
        */
    }

    protected virtual void MoveWeapon()
    {
        if (!_playerDetected) return;

        Vector2 direction = _player.position - _rigidbody.position;
        _referencePoint.up = direction;

        Vector3 rotation = _referencePoint.localEulerAngles;
        rotation.z += 90;
        _referencePoint.localEulerAngles = rotation;
    }

    protected virtual void Attack()
    {
        if (!_canAttack) return;

        if (_timer - _lastTimeShot > _weaponController.WeaponBase.fireRate)
        {
            _animator.SetTrigger("Shoot");
            _isShooting = true;
            _lastTimeShot = _timer;
        }

        _rigidbody.velocity = Vector2.zero;

        if (_player.position.x < _rigidbody.position.x)
        {
            _spriteRenderer.flipX = true;
        }
        else
        {
            _spriteRenderer.flipX = false;
        }
    }

    protected virtual void Shoot()
    {
        _weaponController.Shoot(_player.position - _rigidbody.position);
    }

    /*
    private bool IsInGoalPosition()
    {
        if (Vector2.Distance(transform.position, _pathToTake[0]) < 0.25f)
        {
            return true;
        }
        else return false;
    }
    */

    public bool IsShooting { get { return _isShooting; } set { _isShooting = value; } }
}