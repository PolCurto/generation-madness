using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private Vector2 _direction;
    private float _speed;
    private float _damage;
    private float _timeToLive;

    void FixedUpdate()
    {
        Move();
    }

    /// <summary>
    /// Set all the bullet parameters
    /// </summary>
    public void SetParameters(Vector2 direction, float speed, float damage, float timeToLive, Sprite sprite)
    {
        _direction = direction.normalized;
        _speed = speed;
        _damage = damage;
        _timeToLive = timeToLive;
        _spriteRenderer.sprite = sprite;

        transform.up = _direction;
    }

    /// <summary>
    /// Moves towards the stablished direction
    /// </summary>
    private void Move()
    {
        _rb.velocity = _speed * Time.deltaTime * _direction;
    }
}
