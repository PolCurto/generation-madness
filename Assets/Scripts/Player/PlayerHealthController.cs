using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthController : HealthController
{
    [SerializeField] protected int _currentMaxLife;

    protected override void Start()
    {
        _currentLife = _currentMaxLife;
        UIController.Instance.UpdateLife(_currentMaxLife, _currentLife);
    }

    public override void GetHit(int damage)
    {
        base.GetHit(damage);

        UIController.Instance.UpdateLife(_currentMaxLife, _currentLife);        
    }

    protected override void Die()
    {
        base.Die();
    }

    public override void Heal(int health)
    {
        base.Heal(health);

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
}
