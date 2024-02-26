using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicGunEnemy : Enemy
{
    /*
    void Start()
    {
        
    }
    */
    
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
        if (!_playerDetected || DistanceToPlayer() < _attackDistance) return;

        _direction = _pathToTake[0] - _rigidbody.position;
        Vector2 moveForce = _direction.normalized * _velocity;
        _rigidbody.velocity = moveForce;
        
        if (IsInPosition())
        {
            Debug.Log("We're rich");
            _pathToTake.RemoveAt(0);
            _direction = _pathToTake[0];
        }
    }

    private bool IsInPosition()
    {
        if (_rigidbody.position.magnitude >= _pathToTake[0].magnitude - 1f && _rigidbody.position.magnitude <= _pathToTake[0].magnitude + 1f)
        {
            return true;
        }
        else return false;
    }
}