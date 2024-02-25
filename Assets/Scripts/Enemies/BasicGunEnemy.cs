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

        Vector2 moveForce = _direction.normalized * _velocity;
        _rigidbody.velocity = moveForce;

        
        if (_rigidbody.position == _pathToTake[0])
        {
            _pathToTake.RemoveAt(0);
            _direction = _pathToTake[0];
        }
        
    }
}