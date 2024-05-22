using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthController : HealthController
{
    [SerializeField] protected int _currentMaxLife;
    private PlayerController _playerController;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
    }

    public override void GetHit(int damage)
    {
        if (!_playerController.IsVulnerable) return;

        _playerController.GetHit();
        base.GetHit(damage);

        UIController.Instance.UpdateLife(_currentMaxLife, _currentLife);        
    }

    protected override void Die()
    {
        base.Die();
        _playerController.OnDeath();
    }

    public override void Heal(int health)
    {
        base.Heal(health);

        if (_currentLife > _currentMaxLife)
        {
            _currentLife = _currentMaxLife;
        }

        UIController.Instance.UpdateLife(_currentMaxLife, _currentLife);
    }

    /// <summary>
    /// Modifies the player's max health
    /// </summary>
    /// <param name="health">Health points to modify</param>
    public void ModifyMaxHealth(int health)
    {
        _currentMaxLife += health;

        if (_currentMaxLife > _maxLife)
        {
            _currentMaxLife = _maxLife;
        }

        UIController.Instance.UpdateLife(_currentMaxLife, _currentLife);
    }

    public void SetHealth(int maxHealth, int currentHealth)
    {
        _currentMaxLife = maxHealth;
        _currentLife = currentHealth;

        UIController.Instance.UpdateLife(_currentMaxLife, _currentLife);
    }

    public int MaxLife => _currentMaxLife;
    public int CurrentLife => _currentLife;
}
