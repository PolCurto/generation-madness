using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rb;

    private Vector2 _direction;
    private float _speed;
    private float _damage;
    private float _timeToLive;

    void FixedUpdate()
    {
        Move();
    }

    public void SetParameters(Vector2 direction, float speed, float damage, float timeToLive)
    {
        _direction = direction.normalized;
        _speed = speed;
        _damage = damage;
        _timeToLive = timeToLive;
    }

    private void Move()
    {
        _rb.velocity = _speed * Time.deltaTime * _direction;
    }
}
