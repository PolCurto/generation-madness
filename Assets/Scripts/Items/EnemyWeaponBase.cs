using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Enemy Weapon")]
public class EnemyWeaponBase : ScriptableObject
{
    [Header("Weapon")]
    public new string name;

    public Sprite weaponSprite;
    public float fireRate;
    public float dispersion;    

    [Header("Bullets")]
    public Sprite bulletSprite;
    public float bulletSpeed;
    public int bulletDamage;
    public float bulletDuration;
    public int bulletsPerShot;
}
