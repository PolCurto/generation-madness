using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class Weapon : InventoryItem {

    public Sprite bulletSprite;

    public float fireRate;
    public float bulletSpeed;
    public float bulletDamage;
    public float bulletDuration;
    public float bulletsPerShot;

    public float cameraOffsetMultiplier;
}
