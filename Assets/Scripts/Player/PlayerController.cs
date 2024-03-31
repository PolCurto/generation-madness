using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Camera _camera;
    [SerializeField] private float _interactionBuffer;

    [Header("Movement Parameters")]
    [SerializeField] private float _maxVelocity;
    [SerializeField] private float _acceleration;

    [Header("Weapons")]
    [SerializeField] private Transform _referencePoint;
    [SerializeField] private ActiveWeaponController _activeWeapon;

    private float _timer;
    private Vector2 _playerInput;
    private Vector2 _mousePosition;

    private WeaponsInventory _weaponsInventory;

    public bool DesiredInteraction { get; set; }


    void Awake()
    {
        _weaponsInventory = GetComponent<WeaponsInventory>();
    }

    private void Start()
    {
        _activeWeapon.SwapWeapon((Weapon)_weaponsInventory.GetItem(0));
    }

    void Update()
    {
        MoveWeapon();
        HandleInputs();
        _timer += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        Move();
    }

    #region Inputs
    /// <summary>
    /// Handles the player's inputs
    /// </summary>
    private void HandleInputs()
    {
        // Movement
        _playerInput.x = Input.GetAxisRaw("Horizontal");
        _playerInput.y = Input.GetAxisRaw("Vertical");
        _playerInput.Normalize();

        // Pause
        if (Input.GetKeyDown(KeyCode.Escape) && UIController.Instance != null)
        {
            UIController.Instance.TogglePauseMenu();
        }

        // Interact
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!DesiredInteraction)
            {
                DesiredInteraction = true;
                Invoke(nameof(ResetInteraction), _interactionBuffer);
            }
        }

        // Shoot
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Debug.Log("Shoot");
            Shoot();
        }

        // Weapons
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ActivateWeaponAtIndex(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ActivateWeaponAtIndex(1);
        }

        _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void ResetInteraction()
    {
        DesiredInteraction = false;
    }

    /// <summary>
    /// Enables the weapon at the given index and disables the rest
    /// </summary>
    /// <param name="index"></param>
    private void ActivateWeaponAtIndex(int index)
    {
        _activeWeapon.SwapWeapon((Weapon)_weaponsInventory.GetItem(index));
    }
    #endregion

    #region Movement
    /// <summary>
    /// Moves the player
    /// </summary>
    private void Move()
    {
        Vector2 moveForce = Vector2.MoveTowards(_rigidbody.velocity, _playerInput * _maxVelocity, _acceleration * Time.fixedDeltaTime);
        _rigidbody.velocity = moveForce;

        if (_rigidbody.velocity.x < 0)
        {
            _spriteRenderer.flipX = true;
        }
        else if (_rigidbody.velocity.x > 0)
            {
            _spriteRenderer.flipX = false;
        }
    }
    #endregion

    #region Weapons
    /// <summary>
    /// Rotates the weapon to point towards the mouse
    /// </summary>
    private void MoveWeapon()
    {
        Vector2 direction = _mousePosition - _rigidbody.position;
        _referencePoint.up = direction;

        Vector3 rotation = _referencePoint.localEulerAngles;
        rotation.z += 90;
        _referencePoint.localEulerAngles = rotation;
        //_referencePoint.eulerAngles.
    }

    /// <summary>
    /// Adds a weapon to the inventory
    /// </summary>
    /// <param name="weapon">Weapon to add</param>
    public void AddWeapon(Weapon weapon)
    {
        _weaponsInventory.AddItem(weapon);
        _activeWeapon.SwapWeapon(weapon);
    }

    private void Shoot()
    {
        _activeWeapon.Shoot();
    }
    #endregion

    public ActiveWeaponController ActiveWeapon => _activeWeapon;

}
