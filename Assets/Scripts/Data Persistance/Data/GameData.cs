using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int _currentMaxLife;
    public int _currentLife;
    public float _damageMultiplier;
    public float _attackSpeed;
    public float _reloadSpeed;
    public float _bulletSpeed;

    public int[] _clipBullets;
    public int[] _totalBullets;
    public int[] _weaponId;
}
