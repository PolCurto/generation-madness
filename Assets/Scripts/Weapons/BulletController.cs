using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private Vector2 _direction;
    private float _speed;
    private int _damage;
    private float _timeToLive;
    private float _timeAlive;

    #region Unity Methods
    private void Update()
    {
        CheckDuration();
    }

    void FixedUpdate()
    {
        Move();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall")) Destroy(gameObject);

        if (collision.TryGetComponent(out HealthController health))
        {
            health.GetHit(_damage);
            Destroy(gameObject);
        }
    }
    #endregion

    /// <summary>
    /// Set all the bullet parameters
    /// </summary>
    public void SetParameters(Vector2 direction, float speed, int damage, float timeToLive, Sprite sprite)
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
        _rb.velocity = _speed * Time.fixedDeltaTime * _direction;
    }

    private void CheckDuration()
    {
        _timeAlive += Time.deltaTime;

        if (_timeAlive > _timeToLive)
        {
            Destroy(gameObject);
        }
    }
}
