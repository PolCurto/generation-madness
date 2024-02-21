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
        //MoveAround();
        FollowPlayer();
    }

    private void FollowPlayer()
    {
        if (!_playerDetected) return;

        Vector2 direction = (Vector2)_player.position - _rigidbody.position;
        Vector2 moveForce = direction.normalized * _velocity * Time.deltaTime * 100;
        _rigidbody.velocity = moveForce;
    }
}
