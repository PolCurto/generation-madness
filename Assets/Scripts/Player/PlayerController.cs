using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private float _interactionBuffer;

    [Header("Movement Parameters")]
    [SerializeField] private float _maxVelocity;
    [SerializeField] private float _acceleration;

    [Header("Weapons")]
    [SerializeField] private Transform _referencePoint;
    [SerializeField] private ActiveWeaponController _activeWeapon;

    private bool _shootInput;
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
        if (_weaponsInventory.TryGetItem(0, out Item item))
        {
            _activeWeapon.SwapWeapon((Weapon)item);
        }
    }

    void Update()
    {
        HandleInputs();
        Shoot();

        _timer += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        Move();
        MoveWeapon();
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
        _shootInput = Input.GetKey(KeyCode.Mouse0);

        // Reload
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
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
        if (_weaponsInventory.TryGetItem(index, out Item item))
        {
            _activeWeapon.SwapWeapon((Weapon)item);
            UIController.Instance.HighlightWeaponAtIndex(index);
        }
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
    /// <param name="weaponBase">Weapon to add</param>
    public void AddWeapon(Weapon weapon)
    {
        _weaponsInventory.AddItem(weapon);
        _activeWeapon.SwapWeapon(weapon);

        UIController.Instance.UpdateWeaponAtIndex(ActiveWeaponIndex, weapon);
    }

    private void Shoot()
    {
        if (_shootInput)
        _activeWeapon.Shoot(_mousePosition - _rigidbody.position);
    }

    private void Reload()
    {
        _activeWeapon.Reload();
    }
    #endregion

    public ActiveWeaponController ActiveWeapon => _activeWeapon;
    public Vector2 MousePosition => _mousePosition;
    public int ActiveWeaponIndex => _weaponsInventory.GetActiveWeaponIndex();

}
