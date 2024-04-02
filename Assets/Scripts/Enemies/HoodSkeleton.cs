using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoodSkeleton : Enemy
{
    [Header("Basic Enemy Parameters")]
    [SerializeField] private float _fireRate;

    /*
    void Start()
    {
        
    }
    */
    
    protected override void Update()
    {
        base.Update();
        Attack();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        FollowPlayer();
    }

    private void FollowPlayer()
    {
        // Remove isAttacking
        if (!_playerDetected || _isAttacking) return;
        
        _direction = (_player.position - _rigidbody.position).normalized;           
        Vector2 moveForce = Vector2.MoveTowards(_rigidbody.velocity, _direction.normalized * _maxVelocity, _acceleration * Time.fixedDeltaTime);
        _rigidbody.velocity = moveForce;

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

    protected void Attack()
    {
        if (!_isAttacking) return;

        _rigidbody.velocity = Vector2.zero;
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
}