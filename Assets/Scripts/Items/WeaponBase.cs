using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class WeaponBase : ScriptableObject
{
    [Header("Weapon")]
    public new string name;
    public Sprite weaponSprite;
    public float fireRate;
    public int maxBullets;
    public int clipSize;
    public float cameraOffsetMultiplier;

    [Header("Bullets")]
    public Sprite bulletSprite;
    public float bulletSpeed;
    public float bulletDamage;
    public float bulletDuration;
    public float bulletsPerShot;
}
