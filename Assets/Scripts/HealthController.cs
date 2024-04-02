using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    [Header("Common parameters")]
    [SerializeField] protected int _maxLife;

    protected int _currentLife;

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

    }
}
