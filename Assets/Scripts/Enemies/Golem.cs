using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem : HoodSkeleton
{
    protected override void Attack()
    {
        if (!_canAttack) return;

        Debug.Log("Golem attacking");

        if (_timer - _lastTimeShot > _weaponController.WeaponBase.fireRate)
        {
            _animator.SetTrigger("Shoot");
            _isShooting = true;
            _lastTimeShot = _timer;
        }

        _rigidbody.velocity = Vector2.zero;

        if (_player.position.x < _rigidbody.position.x)
        {
            _spriteRenderer.flipX = true;
        }
        else
        {
            _spriteRenderer.flipX = false;
        }
    }

    protected override void MoveWeapon()
    {
    }

    public override void Die()
    {
        base.Die();

        FloorLogicController.Instance.OnBossDeath(transform.position);
    }
}
