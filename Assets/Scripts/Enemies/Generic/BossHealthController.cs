using UnityEngine;

public class BossHealthController : HealthController
{
    protected override void Die()
    {
        base.Die();
        gameObject.SetActive(false);
        FloorLogicController.Instance.OnBossDeath(transform.position);
    }
}