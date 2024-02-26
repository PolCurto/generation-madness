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
        
        if (IsInGoalPosition())
        {
            _direction = _pathToTake[1];
            _pathToTake.RemoveAt(0);
        }
    }

    private bool IsInGoalPosition()
    {
        if (Vector2.Distance(transform.position, _pathToTake[0]) < 0.75f)
        {
            return true;
        }
        else return false;
    }
}