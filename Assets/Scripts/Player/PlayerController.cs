using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody;

    [Header("Movement Parameters")]
    [SerializeField] private float _maxVelocity;
    [SerializeField] private float _acceleration;

    private Vector2 _direction;

    void Start()
    {
        
    }

    void Update()
    {
        HandleInputs();
    }

    private void FixedUpdate()
    {
        Move();
    }

    /// <summary>
    /// Handles the player's inputs
    /// </summary>
    private void HandleInputs()
    {
        if (Input.GetKeyDown(KeyCode.W)) _direction.y += 1;
        if (Input.GetKeyUp(KeyCode.W)) _direction.y -= 1;

        if (Input.GetKeyDown(KeyCode.A)) _direction.x -= 1;
        if (Input.GetKeyUp(KeyCode.A)) _direction.x += 1;

        if (Input.GetKeyDown(KeyCode.S)) _direction.y -= 1;
        if (Input.GetKeyUp(KeyCode.S)) _direction.y += 1;

        if (Input.GetKeyDown(KeyCode.D)) _direction.x += 1;
        if (Input.GetKeyUp(KeyCode.D)) _direction.x -= 1;
    }

    /// <summary>
    /// Moves the player
    /// </summary>
    private void Move()
    {
        Vector2 newVelocity = Vector2.MoveTowards(_rigidbody.velocity, _maxVelocity * _direction, _acceleration);

        if (Mathf.Abs(_direction.x) + Mathf.Abs(_direction.y) == 2) newVelocity *= 0.8f;

        _rigidbody.velocity = newVelocity;
    }
}
