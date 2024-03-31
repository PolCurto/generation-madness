using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private Camera _camera;

    [Header("Movement Parameters")]
    [SerializeField] private float _maxVelocity;
    [SerializeField] private float _acceleration;

    [SerializeField] private GameObject _weapon;

    private Rigidbody2D _referencePoint;
    private Vector2 _playerInput;
    private Vector2 _mousePosition;

    void Awake()
    {
        _referencePoint = _weapon.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleInputs();
    }

    private void FixedUpdate()
    {
        Move();
        MoveWeapon();
    }

    /// <summary>
    /// Handles the player's inputs
    /// </summary>
    private void HandleInputs()
    {
        _playerInput.x = Input.GetAxisRaw("Horizontal");
        _playerInput.y = Input.GetAxisRaw("Vertical");
        _playerInput.Normalize();

        if (Input.GetKeyDown(KeyCode.Escape) && UIController.Instance != null)
        {
            UIController.Instance.TogglePauseMenu();
        }

        _mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
    }

    /// <summary>
    /// Moves the player
    /// </summary>
    private void Move()
    {
        Vector2 moveForce = Vector2.MoveTowards(_rigidbody.velocity, _playerInput * _maxVelocity, _acceleration * Time.fixedDeltaTime);
        _rigidbody.velocity = moveForce;
    }

    #region Weapons
    private void MoveWeapon()
    {
        Vector2 direction = _mousePosition - _rigidbody.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _referencePoint.rotation = angle;
    }

    public void SwapWeapon()
    {

    }
    #endregion
}
