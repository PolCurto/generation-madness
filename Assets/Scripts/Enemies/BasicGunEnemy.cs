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
        if (!_playerDetected || _isAttacking) return;

        _direction = _pathToTake[0] - _rigidbody.position;
        Vector2 moveForce = _direction.normalized * _velocity;
        _rigidbody.velocity = moveForce;
        
        if (IsInPosition())
        {
            _pathToTake.RemoveAt(0);
            _direction = _pathToTake[0];
        }
    }

    private bool IsInPosition()
    {
        if (_rigidbody.position.magnitude >= _pathToTake[0].magnitude - 0.5f && _rigidbody.position.magnitude <= _pathToTake[0].magnitude + 0.5f)
        {
            Debug.Log("Enemy magnitude: " + _rigidbody.position.magnitude);
            Debug.Log("Position magnitude: " + _pathToTake[0].magnitude);
            return true;
        }
        else return false;
    }
}