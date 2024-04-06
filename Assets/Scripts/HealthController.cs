using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    [Header("Common parameters")]
    [SerializeField] protected int _maxLife;
    [SerializeField] protected int _currentMaxLife;

    protected int _currentLife;

    protected virtual void Start()
    {
        _currentLife = _currentMaxLife;
    }

    public virtual void GetHit(int damage)
    {
        _currentLife -= damage;

        if (_currentLife <= 0)
        {
            Die();
        }

    }

    protected virtual void Die()
    {

    }

    /// <summary>
    /// Heals the entity
    /// </summary>
    /// <param name="health">Amount to heal</param>
    public virtual void Heal(int health)
    {
        _currentLife += health;
    }
}
