using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody;

    [Header("Movement Parameters")]
    [SerializeField] private float _velocity;

    private Vector2 _playerInput;

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
        _playerInput.x = Input.GetAxisRaw("Horizontal");
        _playerInput.y = Input.GetAxisRaw("Vertical");
        _playerInput.Normalize();
    }

    /// <summary>
    /// Moves the player
    /// </summary>
    private void Move()
    {
        Vector2 moveForce = _playerInput * _velocity * Time.deltaTime * 100;
        _rigidbody.velocity = moveForce;
    }
}
