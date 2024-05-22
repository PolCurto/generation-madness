using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthController : HealthController
{
    [SerializeField] Enemy _parent;

    private void Start()
    {
        _currentLife = _maxLife;
    }

    public override void GetHit(int damage)
    {
        _parent.GetHit();
        base.GetHit(damage);
    }

    protected override void Die()
    {
        base.Die();
        _parent.Die();
    }
}