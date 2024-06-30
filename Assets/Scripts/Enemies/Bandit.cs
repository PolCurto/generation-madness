using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bandit : HoodSkeleton
{
    [Header("Bandit Parameters")]
    [SerializeField] private float _shotOffset;
    protected override void Shoot()
    {
        Vector2 direction = (_player.position + (_player.velocity.normalized * _shotOffset)) - _rigidbody.position;
        _weaponController.Shoot(direction);
        _audioSource.PlayOneShot(_shootAudio);
    }
}
