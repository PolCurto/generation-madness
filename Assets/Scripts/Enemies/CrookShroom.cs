using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrookShroom : HoodSkeleton
{
    [Header("Crook Shroom Parameters")]
    [SerializeField] float _velocityMultiplier;
    private bool _velocityBoosted;
    protected override void FollowPlayer()
    {
        base.FollowPlayer();

        if (!_velocityBoosted)
        {
            _velocityBoosted = true;
            _rigidbody.velocity *= _velocityMultiplier;
        }
       
    }
}
