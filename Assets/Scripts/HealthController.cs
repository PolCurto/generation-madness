using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    [Header("Common parameters")]
    [SerializeField] protected int _maxLife;

    protected int _currentLife;
    protected bool _isDead;

    protected virtual void Start()
    {
        _currentLife = _maxLife;
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
        _isDead = true;
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
