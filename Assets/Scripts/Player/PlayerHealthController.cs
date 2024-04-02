using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthController : HealthController
{
    protected override void Start()
    {
        base.Start();
        UIController.Instance.UpdateLife(_maxLife, _currentLife);
    }

    public override void GetHit(int damage)
    {
        base.GetHit(damage);

        UIController.Instance.UpdateLife(_maxLife, _currentLife);        
    }

    protected override void Die()
    {
        base.Die();
    }
}
