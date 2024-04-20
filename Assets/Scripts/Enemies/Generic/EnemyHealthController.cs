using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthController : HealthController
{
    protected override void Die()
    {
        base.Die();
        gameObject.SetActive(false);
    }
}