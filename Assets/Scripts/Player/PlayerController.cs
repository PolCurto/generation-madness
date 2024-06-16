using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDataPersistance
{
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private PlayerHealthController _healthController;
    [SerializeField] private float _interactionBuffer;
    [SerializeField] private GameObject _interactionIndicator;

    [Header("Movement Parameters")]
    [SerializeField] private float _maxVelocity;
    [SerializeField] private float _acceleration;

    [Header("Weapons")]
    [SerializeField] private Transform _referencePoint;
    [SerializeField] private ActiveWeaponController _activeWeapon;

    private bool _shootInput;
    private float _timer;
    private Vector2 _playerInput;
    private bool _controlsEnabled;

    private InventoryController _itemsInventory;
    private WeaponsInventory _weaponsInventory;

    // Stats
    public float DamageMultiplier { get; private set; }
    public float AttackSpeed { get; private set; }
    public float ReloadSpeed { get; private set; }
    public float BulletSpeed { get; private set; }

    public bool DesiredInteraction { get; set; }
    public bool IsVulnerable { get; set; }

    #region Unity Methods
    void Awake()
    {
        IsVulnerable = true;
        _weaponsInventory = GetComponent<WeaponsInventory>();
        _controlsEnabled = false;
        _spriteRenderer.material = new Material(_spriteRenderer.material);
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
    #endregion

    #region Data Persistance
    public void LoadData(GameData data)
    {
        _healthController.SetHealth(data.currentMaxLife, data.currentLife);
        _maxVelocity = data.maxVelocity;
        DamageMultiplier = data.damageMultiplier;
        AttackSpeed = data.attackSpeed;
        ReloadSpeed = data.reloadSpeed;
        BulletSpeed = data.bulletSpeed;

        UIController.Instance.UpdateStats(_maxVelocity, DamageMultiplier, AttackSpeed, ReloadSpeed, BulletSpeed);

        for (int i = 0; i < data.weaponId.Count; i++)
        {
            _weaponsInventory.AddWeapon(data.weaponId[i], data.clipBullets[i], data.totalBullets[i], i);
        }

        if (data.weaponId.Count > 1)
        {
            UIController.Instance.ShowSecondWeapon();
        }

        if (_weaponsInventory.TryGetItem(0, out Item item))
        {
            _activeWeapon.SwapWeapon((Weapon)item);
        }
    }

    public void SaveData(ref GameData data)
    {
        data.currentMaxLife = _healthController.MaxLife;
        data.currentLife = _healthController.CurrentLife;
        data.maxVelocity = _maxVelocity;
        data.damageMultiplier = DamageMultiplier;
        data.attackSpeed = AttackSpeed;
        data.reloadSpeed = ReloadSpeed;
        data.bulletSpeed = BulletSpeed;

        List<Item> weapons = _weaponsInventory.GetAllItems();

        for (int i = 0; i < weapons.Count; i++ )
        {
            Weapon weapon = (Weapon)weapons[i];

            if (data.weaponId.Count > i)
            {
                data.weaponId[i] = weapon.WeaponBase.id;
                data.clipBullets[i] = weapon.ClipBullets;
                data.totalBullets[i] = weapon.TotalBullets;
            }
            else 
            {
                data.weaponId.Add(weapon.WeaponBase.id);
                data.clipBullets.Add(weapon.ClipBullets);
                data.totalBullets.Add(weapon.TotalBullets);
            }           
        }
    }
    #endregion

    #region Inputs
    /// <summary>
    /// Handles the player's inputs
    /// </summary>
    private void HandleInputs()
    {
        if (!_controlsEnabled) return;

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
    }

    private void ResetInteraction()
    {
        DesiredInteraction = false;
    }

    public void EnableControls()
    {
        _controlsEnabled = true;
    }

    public void DisableControls()
    {
        _controlsEnabled = false;
    }

    /// <summary>
    /// Enables the weapon at the given index and disables the rest
    /// </summary>
    /// <param name="index"></param>
    private void ActivateWeaponAtIndex(int index)
    {
        if (_weaponsInventory.TryGetItem(index, out Item item))
        {
            Debug.Log("Item found");
            _activeWeapon.SwapWeapon((Weapon)item);
            UIController.Instance.HighlightWeaponAtIndex(index, _weaponsInventory.GetAllItems().Count);
        }
    }

    public void CanInteract(bool can)
    {
        if (can)
        {
            _interactionIndicator.SetActive(true);
        }
        else
        {
            _interactionIndicator.SetActive(false);
        }
    }
    #endregion

    #region Movement
    /// <summary>
    /// Applies forces to the player rigidbody in order to move it
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
        Vector2 direction = CursorController.Instance.MousePosition - _rigidbody.position;
        _referencePoint.up = direction;

        Vector3 rotation = _referencePoint.localEulerAngles;
        rotation.z += 90;
        _referencePoint.localEulerAngles = rotation;
    }

    /// <summary>
    /// Adds a weapon to the inventory
    /// </summary>
    /// <param name="weaponBase">Weapon to add</param>
    public void AddWeapon(Weapon weapon)
    {
        if (_weaponsInventory.GetAllItems().Count <= 1)
        {
            UIController.Instance.HighlightWeaponAtIndex(1, _weaponsInventory.GetAllItems().Count + 1);
        }

        _weaponsInventory.AddItem(weapon);
        _activeWeapon.SwapWeapon(weapon);

        UIController.Instance.UpdateWeaponAtIndex(ActiveWeaponIndex, weapon);
    }

    private void Shoot()
    {
        if (_shootInput)
        _activeWeapon.Shoot((CursorController.Instance.MousePosition - _rigidbody.position).normalized);
    }

    private void Reload()
    {
        _activeWeapon.StartReload();
    }
    #endregion

    #region Items
    public void AddItem(Item item)
    {
        _itemsInventory.AddItem(item);
    }

    public void ModifyStats(ItemBase item)
    {
        ModifyMaxHealth(item.life);
        if (item.healOnObtain) Heal(item.life);

        _maxVelocity += item.speed;
        DamageMultiplier += item.attack;
        AttackSpeed += item.attackSpeed;
        ReloadSpeed += item.reloadSpeed;
        BulletSpeed += item.bulletSpeed;

        UIController.Instance.UpdateStats(_maxVelocity, DamageMultiplier, AttackSpeed, ReloadSpeed, BulletSpeed);

        /*
        Debug.Log("Damage: " + DamageMultiplier);
        Debug.Log("Speed: " + _maxVelocity);
        Debug.Log("Attack speed: " + AttackSpeed);
        Debug.Log("Reload speed: " + ReloadSpeed);
        Debug.Log("Bullet speed: " + BulletSpeed);
        */
    }

    public void RestoreAmmo(int amount)
    {
        _activeWeapon.RestoreAmmo(amount);
    }
    #endregion

    #region Health

    public void GetHit()
    {
        IsVulnerable = false;
        StartCoroutine(HitVisualFeedback());
        StartCoroutine(CameraShaker.Instance.ShakeCamera(.3f, .4f));
    }

    private IEnumerator HitVisualFeedback()
    {
        _spriteRenderer.material.SetFloat("_FlashAmount", 1);

        yield return new WaitForSeconds(0.05f);

        _spriteRenderer.material.SetFloat("_FlashAmount", 0);

        for (int i = 0; i < 5; i++)
        {
            Color initialColor = _spriteRenderer.material.color;
            Color finalColor = new Color(initialColor.r, initialColor.g, initialColor.b, 0f);

            float elapsedTime = 0f;
            float fadeDuration = 0.15f;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                _spriteRenderer.material.color = Color.Lerp(initialColor, finalColor, elapsedTime / fadeDuration);
                yield return null;
            }

            initialColor = _spriteRenderer.material.color;
            finalColor = new Color(initialColor.r, initialColor.g, initialColor.b, 1f);
            elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                _spriteRenderer.material.color = Color.Lerp(initialColor, finalColor, elapsedTime / fadeDuration);
                yield return null;
            }
        }

        IsVulnerable = true;
    }

    /// <summary>
    /// Modifies the player's max health
    /// </summary>
    /// </summary>
    /// <param name="health">Health points to modify</param>
    public void ModifyMaxHealth(int health)
    {
        _healthController.ModifyMaxHealth(health);
    }

    public void Heal(int health)
    {
        _healthController.Heal(health);
    }

    /// <summary>
    /// WHen the player dies, disables the controls and shows the death screen
    /// </summary>
    public void OnDeath()
    {
        DisableControls();
        UIController.Instance.OnDeath();
    }
    #endregion

    public ActiveWeaponController ActiveWeapon => _activeWeapon;
    public int ActiveWeaponIndex => _weaponsInventory.GetActiveWeaponIndex();

}
