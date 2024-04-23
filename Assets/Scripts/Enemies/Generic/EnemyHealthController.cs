using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthController : HealthController
{
    [SerializeField] Enemy _parent;

    protected override void Die()
    {
        base.Die();
        _parent.Die();
    }
}